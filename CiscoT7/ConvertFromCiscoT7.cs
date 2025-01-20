// Copyright (c) 2025 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;

namespace RhubarbGeekNz.CiscoT7
{
    [Cmdlet(VerbsData.ConvertFrom, "CiscoT7")]
    [OutputType(typeof(SecureString))]
    public sealed class ConvertFromCiscoT7 : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public IEnumerable<char> InputString;

        protected override void ProcessRecord()
        {
            try
            {
                IEnumerator<byte> enumerator = new HexEnumerator(InputString.GetEnumerator());

                if (!enumerator.MoveNext()) throw new IncompleteParseException();
                int index = enumerator.Current;

                index = ((index >> 4) * 10) + (index & 0xF);

                if (index < 0 || index > 15) throw new IncompleteParseException();

                SecureString result = new SecureString();

                string KEY = "dsfd;kfoA,.iyewrkldJKDHSUBsgvca69834ncxv9873254k;fg87";

                while (enumerator.MoveNext())
                {
                    result.AppendChar((char)(KEY[index++] ^ enumerator.Current));

                    if (index == KEY.Length)
                    {
                        index = 0;
                    }
                }

                WriteObject(result);
            }
            catch (IncompleteParseException ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, null));
            }
            catch (IndexOutOfRangeException ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, null));
            }
        }
    }

    internal class HexEnumerator : IEnumerator<byte>
    {
        private readonly IEnumerator<char> enumerator;
        private byte current;

        internal HexEnumerator(IEnumerator<char> chars)
        {
            enumerator = chars;
        }

        public byte Current => current;

        object IEnumerator.Current => current;

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (enumerator.MoveNext())
            {
                byte high = ReadNybble(enumerator.Current);
                if (enumerator.MoveNext())
                {
                    current = (byte)(high << 4 | ReadNybble(enumerator.Current));
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        private byte ReadNybble(char c)
        {
            c = char.ToUpper(c);

            if (c < '0') throw new IndexOutOfRangeException();

            if (c > '9')
            {
                if (c < 'A') throw new IndexOutOfRangeException();
                if (c > 'F') throw new IndexOutOfRangeException();
                return (byte)(10 + c - 'A');
            }

            return (byte)(c - '0');
        }
    }
}
