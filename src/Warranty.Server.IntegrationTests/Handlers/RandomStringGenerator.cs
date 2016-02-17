using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warranty.Server.IntegrationTests.Handlers
{
    public class RandomStringGenerator
    {
        private readonly Random _random = new Random();
        private int _capitalsBegin = 65;
        private int _capitalsEnd = 90;
        private int _maxIndex = 51;
        private int _gap = 6;
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private char GetRandomChar()
        {
            var charindex = _random.Next(_maxIndex) + _capitalsBegin;
            if (charindex > _capitalsEnd)
                charindex += _gap;
            return Convert.ToChar(charindex);
        }

        private string GetRandom(int length)
        {
            _stringBuilder.Clear();
            for (var i = 0; i < length; i++)
                _stringBuilder.Append(GetRandomChar());
            return _stringBuilder.ToString();
        }

        public string Get(int length)
        {
            _stringBuilder.Clear();
            for (var i = 0; i < length; i++)
                _stringBuilder.Append(GetRandomChar());
            return _stringBuilder.ToString();
        }
    }
}
