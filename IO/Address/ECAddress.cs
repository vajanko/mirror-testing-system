namespace MTS.IO.Address
{
    class ECAddress
    {
        /// <summary>
        /// (Get/Set) Identifier of group of channels this one belongs to. Group is a view on some collection of
        /// channels. For example one group may be Inputs or Output. This value will be usually 
        /// <paramref name="AdsReservedIndexGroups.SymbolValueByHandle"/>
        /// </summary>
        public int IndexGroup { get; set; }
        /// <summary>
        /// (Get/Set) Address of this channel inside group identified by <paramref name="IndexGroup"/>
        /// </summary>
        public int IndexOffset { get; set; }

        /// <summary>
        /// (Get/Set) Full name of this channel. By this identifier access to twincat io server to this channel
        /// is possible. In Twincat system manager also called - Server Symbol Name
        /// </summary>
        public string FullName { get; set; }
    }
}
