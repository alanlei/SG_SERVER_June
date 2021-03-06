﻿using LocalCommons.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LocalCommons.Network
{
    /// <summary>
    /// Disposable Asynchronus Connection Listener
    /// Author: Raphail
    /// </summary>
    public class AsyncListenerUDP : IDisposable
    {
        private IPEndPoint m_EndPoint;
        private Socket m_Root;
        private Type defined;
        private SocketAsyncEventArgs m_SyncArgs;
        private Dictionary<string, long> m_FloodAttempts;

        /// <summary>
        /// Constructs New AsyncListener
        /// </summary>
        /// <param name="ip">IP On Which We Will Wait For Connections</param>
        /// <param name="port">Port On Which We Will Wait For Connections</param>
        /// <param name="defined">Defined Connection Type Instance Of What We Will Create When Accept Any Connection.</param>
        public AsyncListenerUDP(string ip, int port, Type defined)
        {
            this.defined = defined;
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);
            this.m_EndPoint = point;
            try
            {
                Socket s = new Socket(point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                //s.LingerState.Enabled = false;
                /*#if !MONO
                s.ExclusiveAddressUse = false;
                #endif*/
                s.Bind(point);
                IConnectionUDP con = Activator.CreateInstance(this.defined, s) as IConnectionUDP;
                this.m_Root = s;
            }
            catch (SocketException e)
            {
                Log.Info(e.ToString());
            }

            Log.Info("Installed {0} at {1}", defined.Name, point);
	        this.m_SyncArgs = new SocketAsyncEventArgs();
	        //this.m_SyncArgs.Completed += this.M_SyncArgs_Completed;
	       // this.RunAccept();
	        //his.m_FloodAttempts = new Dictionary<string, long>();
        }

        /// <summary>
        /// Runnning Accept.
        /// </summary>
        private void RunAccept()
        {
            bool result = false;
            do
            {
                try
                {
                    result = !this.m_Root.AcceptAsync(this.m_SyncArgs);
                }
                catch (SocketException ex)
                {
                    Log.Info(ex.ToString());
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    break;
                }

                if (result) this.AcceptProceed(this.m_SyncArgs);

            } while (result);
        }

        /// <summary>
        /// Calls From (Socket) When Accept Done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void M_SyncArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
	        this.AcceptProceed(e);
	        this.RunAccept();
        }

        /// <summary>
        /// Calls When Accept Done and We need create a Connection.
        /// Also Have Flood Control.
        /// </summary>
        /// <param name="e">Event Arguments That Contains Accepted Socket.</param>
        private void AcceptProceed(SocketAsyncEventArgs e)
        {

            if (e.SocketError == SocketError.Success)
            {
                IConnectionUDP con = Activator.CreateInstance(this.defined, e.AcceptSocket) as IConnectionUDP;
                //Main.Set();
            }
            e.AcceptSocket = null;
        }

        #region IDisposable Support

        /// <summary>
        /// Dispose Current Listener.
        /// </summary>
        public void Dispose()
        {
	        this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                Socket sockt = Interlocked.Exchange<Socket>(ref this.m_Root, null);
                if (sockt != null)
                    sockt.Close();
                if (this.m_SyncArgs != null)
                {
	                this.m_SyncArgs.Dispose();
                }
            }
            // free native resources
        }

        #endregion
    }
}
