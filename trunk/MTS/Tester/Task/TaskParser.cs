using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OK.Collections.Generic;

using MTS.IO;
using MTS.Editor;

namespace MTS.Tester
{
    class TaskParser
    {
        private readonly char[] separators = new char[] { ' ', '\t', '\n' };

        private XElement timeline;
        private TestCollection tests;
        private Channels channels;
        private IModule module;
        private TaskScheduler scheduler;

        /// <summary>
        /// Dictionary for storing integer ids of tasks. When parsing configuration file each task gets a unique
        /// integer id so it may be indexed in an array
        /// </summary>
        Dictionary<string, int> taskIds = new Dictionary<string, int>();
        /// <summary>
        /// Collection of relationships between two tasks where the second one requires the first one to be completed
        /// before it can be executed
        /// For each task we hold list of tasks that require this task to be completed
        /// </summary>
        List<GraphEdge> required = new List<GraphEdge>();
        /// <summary>
        /// Collection of relationships between two tasks where the second one disallows the first one to be executed at
        /// the same time
        /// For each task we holds list of tasks that ...
        /// </summary>
        List<GraphEdge> disallowed = new List<GraphEdge>();
        List<Task> allTasks;

        public void ParseTasks(out IOrientedGraph reqGraph, out IGraph disGraph, out List<Task> tasks)
        {
            List<XElement> elements = new List<XElement>(timeline.Elements());
            allTasks = new List<Task>(elements.Count);


            for (int i = 0; i < elements.Count; i++)
            {
                Task t = parseTask(elements[i]);
                if (t != null)
                    allTasks.Add(t);
                // if this task has id defined in xml, will be added to dictionary with its index to tasks list
                parseId(elements[i], allTasks.Count - 1);
            }
            // now all known task ids are in our dictionary

            for (int i = 0; i < elements.Count; i++)
            {
                if (!allTasks[i].Enabled)  // this task won't be executed do not add it to the graph
                    continue;

                parseBehavior(elements[i].Element("behavior"), i);
            }

            reqGraph = new OrientedGraph(allTasks.Count, required.Count);
            reqGraph.Build(required);

            disGraph = new UnorientedGraph();
            disGraph.Build(disallowed);

            tasks = allTasks;
        }
        private Task parseTask(XElement elem)
        {
            if (elem == null) return null;

            switch (elem.Name.LocalName)
            {
                case "set":
                    SetChannels s = new SetChannels();
                    foreach (XElement ch in elem.Elements("channel"))
                        s.AddChannel(module.GetChannel<IDigitalOutput>(ch.Attribute("name").Value),
                                     bool.Parse(ch.Attribute("value").Value));
                    return s;
                case "waitfor":
                    WaitForChannels w = new WaitForChannels();
                    foreach (XElement ch in elem.Elements("channel"))
                        w.AddChannel(module.GetChannel<IDigitalInput>(ch.Attribute("name").Value),
                                     bool.Parse(ch.Attribute("value").Value));
                    return w;
                case "wait":
                    return new Wait(TimeSpan.Parse(elem.Attribute("time").Value).TotalMilliseconds);
                case "if":
                    XElement elseElem = elem.Element("else");
                    if (elseElem != null)
                        elseElem = elseElem.Elements().First();

                    IfChannels i = new IfChannels(scheduler,
                        parseTask(elem.Element("then").Elements().First()),
                        parseTask(elseElem));
                    foreach (XElement ch in elem.Elements("channel"))
                        i.AddChannel(module.GetChannel<IDigitalInput>(ch.Attribute("name").Value),
                                     bool.Parse(ch.Attribute("value").Value));
                    return i;
                case "test":
                    return (TestTask)
                        typeof(TestTask).Assembly
                        .GetType("MTS.Tester." + elem.Attribute("testtype").Value)
                        .GetConstructor(new Type[] { typeof(Channels), typeof(TestValue) })
                        .Invoke(new object[] { channels, tests.GetTest(elem.Attribute("testparam").Value) });
                case "check":
                    // this must be test task
                    return new PresenceTest(channels,
                        tests.GetTest(elem.Attribute("testparam").Value),
                        module.GetChannel<IDigitalInput>(elem.Attribute("channel").Value));
                default: return null;
            }
        }
        private void parseBehavior(XElement bhvElem, int taskId)
        {
            if (bhvElem == null)
                return;
            //parseEnabledIf(bhvElem.Element("enabledif"), taskId);
            if (allTasks[taskId].Enabled)
            {
                parseRequire(bhvElem.Attribute("require"), taskId);
                parseDisallow(bhvElem.Attribute("disallow"), taskId);
            }
        }
        private void parseRequire(XAttribute req, int taskId)
        {
            if (req == null)
                return;
            string[] ids = req.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in ids)
            {
                int i = taskIds[id];
                if (allTasks[i].Enabled)   // this task will be executed, so add it to the graph
                    required.Add(new GraphEdge(i, taskId));
            }
        }
        private void parseDisallow(XAttribute dis, int taskId)
        {
            if (dis == null)
                return;
            string[] ids = dis.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in ids)
            {
                int i = taskIds[id];
                if (allTasks[i].Enabled)   // this task will be executed, so add it to the graph
                    disallowed.Add(new GraphEdge(i, taskId));
            }
        }
        private void parseEnabledIf(XElement enabledElem, int taskId)
        {
            if (enabledElem == null)
                return;

            bool enabled = true;
            foreach (XElement ch in enabledElem.Elements("channel"))
                enabled &= (module.GetChannel<IDigitalInput>(ch.Attribute("name").Value).Value ==
                            bool.Parse(ch.Attribute("value").Value));
            //allTasks[taskId].Enabled = enabled;
        }
        private void parseId(XElement elem, int key)
        {
            XAttribute id = elem.Attribute("id");
            if (id != null)
                taskIds.Add(id.Value, key);
        }

        public TaskParser(XElement timeline, TestCollection tests, Channels channels, TaskScheduler scheduler)
        {
            this.timeline = timeline;
            this.tests = tests;
            this.channels = channels;
            this.module = channels;
            this.scheduler = scheduler;
        }

        class TaskData
        {
            public Task Task;
            public string Id;
            public List<string> Required = new List<string>();

            public TaskData(Task task)
            {
                Task = task;
            }
        }
    }
}
