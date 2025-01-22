// Copyright (c) 2025 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;

namespace RhubarbGeekNz.CiscoT7
{
    [Cmdlet(VerbsData.ConvertTo, "CiscoT7")]
    [OutputType(typeof(String))]
    public class ConvertToCiscoT7 : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public SecureString SecureString;

        protected override void ProcessRecord()
        {
            try
            {
                string HEX = "0123456789ABCDEF";
                int index = new Random().Next(16) & 0xF;
                int length = SecureString.Length << 1;
                char[] stringBuilder = new char[length + 2];

                stringBuilder[0] = HEX[index / 10];
                stringBuilder[1] = HEX[index % 10];

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

                        stringBuilder[2 + i] = HEX[value >> 4];
                        stringBuilder[3 + i] = HEX[value & 0xF];

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

                WriteObject(new String(stringBuilder));

                Array.Clear(stringBuilder, 0, stringBuilder.Length);
            }
            catch (IndexOutOfRangeException ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, null));
            }
        }
    }
}
