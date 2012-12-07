// -----------------------------------------------------------------------
// <copyright file="ISessionManager.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Extensions.WebApi
{
    using System.Data;

    internal interface ISessionManager
    {
        void OnActionExecuted(IReadOnlySession session, bool manageTransaction, bool hasException);

        void OnActionExecuting(IReadOnlySession session, bool manageTransaction, IsolationLevel? isolationLevel);
    }
}