﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BO
{
    public class ToolStringClass
    {
        public static string ToStringProperty<T>(T t)
        {
            string str = "";
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name
        + ": " + item.GetValue(t, null);
            return str;
        }

    }
}