using System;
using System.Diagnostics;
using System.Management;

namespace VirtualDisplay
{
    //класс определяющий есть ли подключение или отключение усилителя
    //коды ошибок 3XXX
    class ComWatcher
    {
        ManagementEventWatcher watchingObect = null;
        WqlEventQuery watcherQuery;
        ManagementScope scope;

        public event EventHandler UsbAddedEventHandler;
        public event EventHandler UsbRemoveEventHandler;

        public ComWatcher()
        {
            scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            AddInsertUSBHandler();
            AddRemoveUSBHandler();
        }

        public void AddRemoveUSBHandler()
        {
            try
            {
                USBWatcherSetUp("__InstanceDeletionEvent");
                watchingObect.EventArrived += USBRemoved;
                watchingObect.Start();
                //watchingObect.Stop();
            }

            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
                Logger.WriteLine(e.Message + ". Код ошибки #3001");
                if (watchingObect != null)
                    watchingObect.Stop();
            }
        }

        void AddInsertUSBHandler()
        {

            try
            {
                USBWatcherSetUp("__InstanceCreationEvent");
                watchingObect.EventArrived += USBAdded;
                watchingObect.Start();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                Logger.WriteLine(e.Message + ". Код ошибки #3002");

                if (watchingObect != null)
                    watchingObect.Stop();

            }

        }

        private void USBWatcherSetUp(string eventType)
        {
            watcherQuery = new WqlEventQuery();
            watcherQuery.EventClassName = eventType;
            watcherQuery.WithinInterval = new TimeSpan(0, 0, 2);
            watcherQuery.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";
            watchingObect = new ManagementEventWatcher(scope, watcherQuery);
        }

        public void USBAdded(object sender, EventArgs e)
        {
            try
            {
                //Debug.WriteLine("A USB device inserted");
                UsbAddedEventHandler?.Invoke(sender, e);
            }
            catch(Exception exc)
            {
                Logger.WriteLine(exc.Message + ". Код ошибки #3003");
            }
        }

        public void USBRemoved(object sender, EventArgs e)
        {
            try
            {
                UsbRemoveEventHandler?.Invoke(sender, e);
                Debug.WriteLine("A USB device removed");
            }
            catch (Exception exc)
            {
                Logger.WriteLine(exc.Message + ". Код ошибки #3004");
            }
        }
    }
}
