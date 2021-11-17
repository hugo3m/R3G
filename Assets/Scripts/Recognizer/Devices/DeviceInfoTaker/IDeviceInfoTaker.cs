namespace Recognizer.DeviceInfoTaker
{
    public interface IDeviceInfoTaker
    {
        void Fill(ref double[] data);
        void FillWithTime(ref double[] data, double timestamp);
    }
}