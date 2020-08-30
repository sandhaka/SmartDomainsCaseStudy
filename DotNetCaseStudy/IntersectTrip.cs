using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DotNetCaseStudy
{
    public class IntersectTrip
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Stopwatch _time = new Stopwatch();

        public IntersectTrip(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Setup();
        }
        
        [Fact]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void Test()
        {
            var intersections = new List<List<int>>(); 
            
            DoTest("Intersect bigger with smaller list", () =>
            {
                intersections.Add(_fooList!.Keys.Intersect(_barList!.Keys).ToList());
            });
            
            DoTest("Intersect smaller with bigger list", () =>
            {
                intersections.Add(_fooList!.Keys.Intersect(_barList!.Keys).ToList());
            });
            
            Assert.NotEmpty(intersections);
            Assert.True(intersections.TrueForAll(i => i.Count == 50000));
        }

        private void DoTest(string x, Action test)
        {
            _time.Start();

            test();
            
            _time.Stop();
            
            _testOutputHelper.WriteLine($"Test {x}: {_time.Elapsed:c}");
            
            _time.Reset();
        }
        
        #region Setup
        
        private static readonly Random Random = new Random();
        
        class Foo
        {
            public int Id { get; set; }
            public string FooName { get; set; }
        }

        class Bar
        {
            public int Id{ get; set; }
            public string BarName { get; set; }
        }

        private Dictionary<int, Foo> _fooList = new Dictionary<int, Foo>();
        private Dictionary<int, Bar> _barList = new Dictionary<int, Bar>();
        
        private void Setup()
        {
            for (var i = 0; i < 100000; i++)
            {
                var foo = new Foo {Id = i, FooName = GenerateString(4)};
                _fooList.Add(foo.Id, foo);
            }
            
            for (var i = 0; i < _fooList.Count / 2; i++)
            {
                var bar = new Bar {Id = _fooList.ElementAt(i).Key, BarName = GenerateString(4)};
                _barList.Add(bar.Id, bar);
            }

            var temp = _fooList.ToList();
            temp.Shuffle();
            _fooList = temp.ToDictionary(i => i.Key, i => i.Value);
        }
        
        #endregion
        
        #region Tools
        
        private const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private string GenerateString(int size)
        {
            char[] chars = new char[size];
            for (int i=0; i < size; i++)
            {
                chars[i] = Alphabet[Random.Next(Alphabet.Length)];
            }
            return new string(chars);
        }

        #endregion
    }
    
    internal static class Extensions
    {
        private static readonly Random Random = new Random();
        
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Random.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
    }
}