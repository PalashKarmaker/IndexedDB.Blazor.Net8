﻿using Microsoft.JSInterop;

namespace IndexedDB.Blazor
{
    public class IndexedDbFactory(IJSRuntime jSRuntime) : IIndexedDbFactory
    {

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> Create<T>() where T : IndexedDb
        {
            if (Activator.CreateInstance(typeof(T), jSRuntime, typeof(T).Name, 1) is not T instance)
                throw new Exception("Instance not created");
            var connected = await instance.WaitForConnection();
            if (!connected)
                throw new Exception("Could not connect");
            return instance;
        }

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="version">IndexedDb version</param>
        /// <returns></returns>
        public async Task<T> Create<T>(int version) where T : IndexedDb
        {
            var instance = (T)Activator.CreateInstance(typeof(T), jSRuntime, typeof(T).Name, version)!;
            var connected = await instance.WaitForConnection();
            if (!connected)
                throw new Exception("Could not connect");
            return instance;
        }

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">IndexedDb name</param>
        /// <returns></returns>
        public async Task<T> Create<T>(string name) where T : IndexedDb
        {
            if (Activator.CreateInstance(typeof(T), jSRuntime, name, 1) is not T instance)
                throw new Exception("Instance not created");
            var connected = await instance.WaitForConnection();

            if (!connected)
                throw new Exception("Could not connect");
            return instance;
        }

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">IndexedDb name</param>
        /// <param name="name">IndexedDb version</param>
        /// <returns></returns>
        public async Task<T> Create<T>(string name, int version) where T : IndexedDb
        {
            var instance = (T)Activator.CreateInstance(typeof(T), jSRuntime, name, version)!;

            var connected = await instance.WaitForConnection();

            if (!connected)
                throw new Exception("Could not connect");

            return instance;
        }
    }
}
