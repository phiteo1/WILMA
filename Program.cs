
using System;
using System.Threading.Tasks;

namespace RPCClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Device device = new Device("72e28c40-3ac6-11ef-8060-371813e8cf1e", "NrufaQS2v0mRvei2Iwo4");
            Task.Factory.StartNew(() =>{ device.daemonDevice(); }).GetAwaiter().GetResult();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
        
    }

}
