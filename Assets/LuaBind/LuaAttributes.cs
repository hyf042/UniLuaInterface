using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaBind
{
    /// <summary>
    /// Marks a method, field or property to be hidden from Lua auto-completion
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class LuaHideAttribute : Attribute
    { }
    /// <summary>
    /// Marks a method for global usage in Lua scripts
    /// </summary>
    /// <see cref="LuaRegistrationHelper.TaggedInstanceMethods"/>
    /// <see cref="LuaRegistrationHelper.TaggedStaticMethods"/>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class LuaGlobalAttribute : Attribute
    {
        /// <summary>
        /// An alternative name to use for calling the function in Lua - leave empty for CLR name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the function
        /// </summary>
        public string Description { get; set; }

        public LuaGlobalAttribute(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}
