using System;
using System.IO.Ports;
using System.Windows;

namespace VirtualDisplay
{
    public class ComPortConnector
    {
        static public SerialPort serialPort1;
        public event Action<byte[]> DataReceivedEvent;


        public string[] LoadComPort()
        {
            string[] ports = SerialPort.GetPortNames();
            return ports;
        }

        public ComPortConnector()
        {
            serialPort1 = new SerialPort();
        }

        public bool OpenPort(string cp_name)
        {
            try
            {
                int BAUDRATE = 9600;
                int DATABITS = 8;
                serialPort1.PortName = cp_name;
                serialPort1.BaudRate = BAUDRATE;
                serialPort1.DataBits = DATABITS;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;


                //обработчик события поступления сигнала от COM порта
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);

                serialPort1.Open();

                return true;
            }            
            catch(Exception ex)
            {
                MessageBox.Show("Отсутствует доступ к COM порту. Код ошибки #3001", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.WriteLine(ex.Message + ". Код ошибки #3001");
                return false;
            }
        }

        public bool IsOpen()
        {
            return serialPort1.IsOpen;
        }

        public void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            
            byte[] bytesIn = new byte[5];

            try
            {
                sp.Read(bytesIn, 0, bytesIn.Length);

                // check that first two bytes begin on FF и EB, if false send null
                if (0xFF == bytesIn[0] & 0xEB == bytesIn[1])
                    DataReceivedEvent?.Invoke(bytesIn);
                else
                    DataReceivedEvent?.Invoke(null);
            }
            catch(Exception ex)
            {
                Logger.WriteLine(ex.Message + ". Код ошибки #3002");
            }
        }

        public bool ClosePort()
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                   serialPort1.Close();
                   return true;
                }
                return false;
                
            }
            catch
            {
                MessageBox.Show("Закрыте COM порта не выполнено","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        //сохраняет только первые 4 бита в байте, которые относятся к заводскому ID кнопки,
        //вторые 4 бита удаляются, так как относятся сервис коду нажатой кнопки
        public int TakeBtnID(byte[] dataIn)
        {
            int a, b, c;

            a = dataIn[2];
            a <<= 16;
            b = dataIn[3];
            b <<= 8;
            c = dataIn[4];
            c >>= 4;

            int btnnumber = a + b + c;
            btnnumber >>= 4;
            
            return btnnumber;
        }

        //Метод берет 5-ый байт и возвращает сервис код нажатой кнопки
        public byte TakePushButtonService(byte dataIn)
        {
            byte servicebyte = (byte)(dataIn & 0x0F);
            return servicebyte;
        }
    }
}
