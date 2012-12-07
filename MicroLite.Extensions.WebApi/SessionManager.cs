// -----------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="MicroLite">
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

    /// <summary>
    /// A class used by the MicroLiteSessionAttribute to manage sessions.
    /// </summary>
    internal sealed class SessionManager : ISessionManager
    {
        public void OnActionExecuted(IReadOnlySession session, bool manageTransaction, bool hasException)
        {
            if (session != null)
            {
                if (manageTransaction && session.Transaction != null)
                {
                    if (hasException)
                    {
                        if (!session.Transaction.WasRolledBack)
                        {
                            session.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        if (session.Transaction.IsActive)
                        {
                            session.Transaction.Commit();
                        }
                    }
                }

                session.Dispose();
            }
        }

        public void OnActionExecuting(IReadOnlySession session, bool manageTransaction, IsolationLevel? isolationLevel)
        {
            if (manageTransaction)
            {
                if (isolationLevel.HasValue)
                {
                    session.BeginTransaction(isolationLevel.Value);
                }
                else
                {
                    session.BeginTransaction();
                }
            }
        }
    }
}