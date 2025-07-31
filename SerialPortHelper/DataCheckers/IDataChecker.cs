namespace SerialPortHelper.DataCheckers
{
    /// <summary>
    /// 数据校验器接口
    /// </summary>
    public interface IDataChecker
    {
        bool Check(byte[] buffer);
    }
}
