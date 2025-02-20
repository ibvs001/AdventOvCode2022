﻿using AdventOfCode2022.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Xml.Linq;

namespace AdventOfCode2022.Days
{
    internal class Day11
    {
        static int year = 2022;
        static int day = 11;
        static string inputPath = $@"D:\Development\AdventOfCode\AdventOfCode{year}\AdventOfCode{year}\Days\Day{day}\Input.txt";
        //static string inputPath = $@"D:\Development\AdventOfCode\AdventOfCode{year}\AdventOfCode{year}\Days\Day{day}\SampleInput.txt";

        static string cookiePath = @"D:\Development\AdventOfCode\cookie.txt";

        static long part1 = 0;
        static long part2 = 0;

        public void Run(int submitPartNumber = -1)
        {
            //var cookie = File.ReadAllText(cookiePath);
            //InputHelper.GetInput(inputPath, year, day, cookie);
            string[] lines = File.ReadAllLines(inputPath);

            SolveProblem(lines);

            try
            {
                //if (submitPartNumber == 1)
                //{
                //    InputHelper.SubmitAnswer(part1, year, day, cookie);
                //}

                //if (submitPartNumber == 2)
                //{
                //    InputHelper.SubmitAnswer(part2, year, day, cookie);
                //}
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to submit!");
                throw;
            }
        }

        private void SolveProblem(string[] lines)
        {
            lines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

            part1 = RunRounds(20, lines, 3);
            part2 = RunRounds(10000, lines, 1);

            Console.WriteLine($"part1: {part1}");
            Console.WriteLine($"part2: {part2}");
        }

        private long RunRounds(int rounds, string[] lines, decimal worryLevelDivider)
        {
            var monkeys = new List<List<string>>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("Monkey"))
                {
                    monkeys.Add(new List<string>());
                    var monkey = line.Substring(line.IndexOf(" ") + 1, 1).Trim();
                    monkeys.Last().Add(monkey);
                    //Console.WriteLine($"add monkey: {monkey}");
                }
                else if (line.Trim().StartsWith("Starting"))
                {
                    var step = line.Substring(line.IndexOf(":") + 1, line.Length - line.IndexOf(":") - 1).Trim();
                    monkeys.Last().Add(step);
                    //Console.WriteLine($"starting items: {step}");
                }
                else if (line.Trim().StartsWith("Operation"))
                {
                    var step = line.Substring(line.IndexOf("=") + 1, line.Length - line.IndexOf("=") - 1).Trim();
                    monkeys.Last().Add(step);
                    //Console.WriteLine($"operation: {step}");
                }
                else if (line.Trim().StartsWith("Test"))
                {
                    var step = line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")).Trim();
                    monkeys.Last().Add(step);
                    //Console.WriteLine($"test: {step}");
                }
                else if (line.Trim().Contains("true"))
                {
                    var step = line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")).Trim();
                    monkeys.Last().Add(step);
                    //Console.WriteLine($"true: {step}");
                }
                else if (line.Trim().Contains("false"))
                {
                    var step = line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")).Trim();
                    monkeys.Last().Add(step);
                    //Console.WriteLine($"false: {step}");

                    // Add inspection indicator
                    monkeys.Last().Add("0");
                }
            }

            var diviser = monkeys.Aggregate(1, (seed, monkey) => seed * Int32.Parse(monkey[3]));

            for (int i = 1; i <= rounds; i++)
            {
                for (int j = 0; j < monkeys.Count; j++)
                {
                    var monkey = monkeys[j];
                    var items = monkey[1].Split(",").ToList();
                    var operationCalc = monkey[2];
                    var operationP1 = operationCalc.Substring(0, operationCalc.IndexOf(" ")).Trim();
                    var operationP2 = operationCalc.Substring(operationCalc.LastIndexOf(" "), operationCalc.Length - operationCalc.LastIndexOf(" ")).Trim();

                    for (int k = 0; k < items.Count; k++)
                    {
                        if (string.IsNullOrWhiteSpace(items[k].Trim()))
                        {
                            break;
                        }

                        monkeys[j][6] = (Int32.Parse(monkeys[j][6]) + 1).ToString();

                        long currentLevel = Int64.Parse(items[k].Trim());
                        long worryLevel = 0;
                        //Console.WriteLine($"currentLevel: {currentLevel}");

                        if (operationCalc.Contains("*"))
                        {
                            worryLevel = GetValue(operationP1, currentLevel) * GetValue(operationP2, currentLevel);
                        }
                        else if (operationCalc.Contains("+"))
                        {
                            worryLevel = GetValue(operationP1, currentLevel) + GetValue(operationP2, currentLevel);
                        }

                        var levelToPass = worryLevelDivider == 3 ? Math.Floor(Math.Abs(worryLevel) / worryLevelDivider) : worryLevel % diviser;

                        //Console.WriteLine($"round: {i}, monkey: {monkey[0]}, item: {currentLevel}, levelToPass: {levelToPass}, worryLevel: {worryLevel}");

                        if (levelToPass % Int64.Parse(monkey[3]) == 0)
                        {
                            //Console.WriteLine($"pass to monkey: {monkey[4]}, levelToPass: {levelToPass}");
                            monkeys[Int32.Parse(monkey[4])][1] += (string.IsNullOrWhiteSpace(monkeys[Int32.Parse(monkey[4])][1]) ? string.Empty : ",") + levelToPass;
                        }
                        else
                        {
                            //Console.WriteLine($"pass to monkey: {monkey[5]}, levelToPass: {levelToPass}");
                            monkeys[Int32.Parse(monkey[5])][1] += (string.IsNullOrWhiteSpace(monkeys[Int32.Parse(monkey[5])][1]) ? string.Empty : ",") + levelToPass;
                        }

                        //Console.WriteLine($"monkey: {monkey[0]}, item length: {items[k].Length}");
                    }

                    monkeys[j][1] = string.Empty;

                    //Console.WriteLine($"items: {string.Join(',', items)}");
                }

                foreach (var monkey in monkeys)
                {
                    if (i % 1000 == 0 || i == 20)
                    {
                        var response = $"round: {i}, monkey: {monkey[0]}, test: {monkey[3]}, items: {string.Join(',', monkey[1])}, inspected: {monkey[6]}";
                        Console.WriteLine(response);
                    }
                }
            }

            return monkeys.Select(m => Int64.Parse(m[6])).OrderByDescending(m => m).Take(2).Aggregate((a, x) => a * x);
        }

        private long GetValue(string operationValue, long currentValue)
        {
            return Math.Abs(Int64.Parse(operationValue.Replace("old", currentValue.ToString())));
        }
    }
}