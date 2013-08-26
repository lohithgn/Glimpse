﻿using System;
using System.Collections.Generic;
using Glimpse.Owin.Middleware;
using Owin;

namespace Glimpse.Owin
{
    public class AppBuilder : IAppBuilder
    {
        private readonly IAppBuilder innerApp;

        public AppBuilder(IAppBuilder app)
        {
            innerApp = app;
            innerApp.UseType<HeadMiddleware>(this); // This is the earliest we can add middleware
        }

        public IDictionary<string, object> Properties
        {
            get { return innerApp.Properties; }
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            innerApp.Use(middleware, args);
            return this;
        }

        public object Build(Type returnType)
        {
            // innerApp.UseType<TailMiddleware>(); // This is the last middleware added
            return innerApp.Build(returnType);
        }

        public IAppBuilder New()
        {
            return new AppBuilder(innerApp.New());
        }
    }
}
