﻿namespace WebApi.Commands.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISubscribeCommand
    {
        /// <summary>
        /// 訂閱資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        void Subscribe<T>(Func<T, bool> func);
    }
}
