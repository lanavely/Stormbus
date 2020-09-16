namespace Stormbus.UI.Command.CommandData
{
    /// <summary>
    ///     Data required to execute the command by modbus client
    /// </summary>
    public class CommandDataBase
    {
        public ushort Address { get; set; }
    }
}