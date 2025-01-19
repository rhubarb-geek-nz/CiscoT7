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

        static private readonly byte[] KEY = {
            0x64, 0x73, 0x66, 0x64, 0x3B, 0x6B, 0x66, 0x6F, 0x41, 0x2C,
            0x2E, 0x69, 0x79, 0x65, 0x77, 0x72, 0x6B, 0x6C, 0x64, 0x4A,
            0x4B, 0x44, 0x48, 0x53, 0x55, 0x42, 0x73, 0x67, 0x76, 0x63,
            0x61, 0x36, 0x39, 0x38, 0x33, 0x34, 0x6E, 0x63, 0x78, 0x76,
            0x39, 0x38, 0x37, 0x33, 0x32, 0x35, 0x34, 0x6B, 0x3B, 0x66,
            0x67, 0x38, 0x37};
    }
}
