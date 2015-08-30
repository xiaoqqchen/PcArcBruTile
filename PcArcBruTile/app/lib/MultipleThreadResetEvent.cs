using System;
using System.Threading;

namespace BrutileArcGIS.Lib
{
    public class MultipleThreadResetEvent : IDisposable
    {
        private readonly ManualResetEvent _done;
        private readonly int _total;
        private long _current;

        /// <summary>  
        /// 构造函数
        /// </summary>  
        /// <param name="total">需要等待执行的线程总数</param>
        public MultipleThreadResetEvent(int total)
        {
            this._total = total;
            _current = total;
            _done = new ManualResetEvent(false);

        }
        /// <summary>  
        /// 唤醒一个等待的线程  
        /// </summary>  
        public void SetOne()
        {
            // Interlocked 原子操作类 ,此处将计数器减1  
            if (Interlocked.Decrement(ref _current) == 0)
            {
                //当所以等待线程执行完毕时，唤醒等待的线程  
                _done.Set();
            }
        }

        /// <summary>  
        /// 等待所以线程执行完毕  
        /// </summary>  
        public void WaitAll()
        {
            _done.WaitOne();
        }

        /// <summary>  
        /// 释放对象占用的空间  
        /// </summary>  
        public void Dispose()
        {
            ((IDisposable)_done).Dispose();
        }
    }
}
