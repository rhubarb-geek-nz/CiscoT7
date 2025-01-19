// Copyright (c) 2025 Roger Brown.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security;

namespace RhubarbGeekNz.CiscoT7
{
    [TestClass]
    public class CiscoT7Tests
    {
        readonly InitialSessionState initialSessionState = InitialSessionState.CreateDefault();

        public CiscoT7Tests()
        {
            foreach (Type t in new Type[] {
                typeof(ConvertFromCiscoT7),
                typeof(ConvertToCiscoT7)
            })
            {
                CmdletAttribute ca = t.GetCustomAttribute<CmdletAttribute>();

                initialSessionState.Commands.Add(new SessionStateCmdletEntry($"{ca.VerbName}-{ca.NounName}", t, ca.HelpUri));
            }

            initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Stop"));
        }

        [TestMethod]
        public void TestConvertToCiscoT7()
        {
            string value = Guid.NewGuid().ToString();
            SecureString s = new SecureString();
            string encoding;

            foreach (char c in value)
            {
                s.AppendChar(c);
            }

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("ConvertTo-CiscoT7").AddParameter("SecureString", s);

                var result = powerShell.Invoke();
                Assert.AreEqual(1, result.Count);
                encoding = (string)result[0].BaseObject;
            }

            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("ConvertFrom-CiscoT7").AddParameter("InputString", encoding);

                var result = powerShell.Invoke();
                Assert.AreEqual(1, result.Count);

                SecureString secure = (SecureString)result[0].BaseObject;
                var password = new System.Net.NetworkCredential(string.Empty, secure).Password;
                Assert.AreEqual(value, password);
            }
        }

        [TestMethod]
        public void TestConvertFromCiscoT7()
        {
            string value = "0600002E4E4F1B";
            using (PowerShell powerShell = PowerShell.Create(initialSessionState))
            {
                powerShell.AddCommand("ConvertFrom-CiscoT7").AddParameter("InputString", value);

                var result = powerShell.Invoke();
                Assert.AreEqual(1, result.Count);

                SecureString secure = (SecureString)result[0].BaseObject;
                var password = new System.Net.NetworkCredential(string.Empty, secure).Password;
                Assert.AreEqual("foobar", password);
            }
        }
    }
}
