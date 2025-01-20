// Copyright (c) 2025 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace RhubarbGeekNz.CiscoT7
{
    [Cmdlet(VerbsData.ConvertTo, "CiscoT7")]
    [OutputType(typeof(String))]
    public class ConvertToCiscoT7 : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public System.Security.SecureString SecureString;

        protected override void ProcessRecord()
        {
            try
            {
                int index = new Random().Next(16) & 0xF;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(index.ToString("D2"));

                int length = SecureString.Length << 1;
                IntPtr valuePtr = SecureStringMarshal.SecureStringToCoTaskMemUnicode(SecureString);

                try
                {
                    string KEY = "dsfd;kfoA,.iyewrkldJKDHSUBsgvca69834ncxv9873254k;fg87";

                    for (int i = 0; i < length; i += 2)
                    {
                        short unicodeChar = Marshal.ReadInt16(valuePtr, i);

                        if ((unicodeChar < ' ') || (unicodeChar > 0x7E))
                        {
                            throw new IndexOutOfRangeException();
                        }

                        byte value = (byte)(KEY[index++] ^ unicodeChar);
                        stringBuilder.Append(value.ToString("X2"));

                        if (index == KEY.Length)
                        {
                            index = 0;
                        }
                    }
                }
                finally
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(valuePtr);
                }

                WriteObject(stringBuilder.ToString());
            }
            catch (IndexOutOfRangeException ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, null));
            }
        }
    }
}
