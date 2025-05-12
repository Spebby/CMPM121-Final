using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CMPM.Utils.Structures;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;


namespace CMPM.Utils {
    // I figured a wrapper for RPN Strings was fitting to make the use case clear & keep associated data together
    // + reduce user error.
    [JsonConverter(typeof(RPNStringParser))]
    public struct RPNString : IEquatable<RPNString> {
        public readonly string String;
        [CanBeNull] public readonly SerializedDictionary<string, int> Variables;

        public RPNString(string str, SerializedDictionary<string, int> vars = null) {
            String = str;
            Variables = vars;
        }
        
        public static implicit operator int(RPNString entry) => RPN.Evaluate(entry.String, entry.Variables);
        public static implicit operator string(RPNString entry) => entry.String;
        
        public static bool operator !=(RPNString a, RPNString b) => a.String != b.String;
        public static bool operator ==(RPNString a, RPNString b) => a.String == b.String;

        public override bool Equals(object obj) => obj is RPNString other && this == other;
        public bool Equals(RPNString other) => String == other.String;
        public override int GetHashCode() => String.GetHashCode();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Evaluate(SerializedDictionary<string, int> variables) => RPN.Evaluate(String, variables);
        public readonly float Evaluate(SerializedDictionary<string, float> variables) => RPN.Evaluate(String, variables);
    }
    
    
    public static class RPN {
        static readonly char[] SUPPORTED_TOKENS = { '+', '-', '*', '/', '%', '^' };

        #region Integers
        public static int Evaluate(in string expression, in SerializedDictionary<string, int> vars) {
            Stack<int> stack = new();
            foreach (string token in expression.Split(' ')) {
                if (SUPPORTED_TOKENS.Contains(token[0])) {
                    // I'm not being super careful w/ error handling due to scope of when we're using this & b/c majority of non-explicit errors will be caught by default case
                    if (stack.Count < 2) {
                        throw new InvalidOperationException(
                            $"RPN {expression} has insufficient operands for operator: {token}");
                    }

                    int a = stack.Pop();
                    int b = stack.Pop();
                    stack.Push(ApplyOperator(token[0], b, a));
                    continue;
                }


                bool success = vars.TryGetValue(token, out int value);
                if (!success) {
                    success = int.TryParse(token, out value);
                }

                // Throw an error if the token is neither a valid operator nor a number/variable
                if (!success) throw new InvalidOperationException($"Invalid token: {token}");
                stack.Push(value);
            }

            if (stack.Count != 1) {
                throw new InvalidOperationException($"{expression} is not a valid RPN expression!");
            }

            return stack.Pop();
        }

        // At some point it may be worth revisiting to add float support, trig, sqrt, etc.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int ApplyOperator(char op, int b, int a) {
            // If we want to be silly, we could use bytes here instead >:)
            return op switch {
                '+'             => b + a,
                '-'             => b - a,
                '*'             => b * a,
                '/' when a == 0 => throw new DivideByZeroException("Division by zero."),
                '/'             => b / a,
                '%'             => b % a,
                '^'             => IntPow(b, a),
                _               => throw new InvalidOperationException($"Unsupported operator: {op}")
            };
        }

        static int IntPow(int x, int p) {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (p == 0) return 1;
            if (p == 1) return x;

            int tmp = IntPow(x, p/2);
            if (p % 2 == 0) return tmp * tmp;
            return x * tmp * tmp;
        }
        #endregion
        
        #region Floats
        public static float Evaluate(in string expression, in SerializedDictionary<string, float> vars) {
            Stack<float> stack = new();
            foreach (string token in expression.Split(' ')) {
                if (SUPPORTED_TOKENS.Contains(token[0])) {
                    // I'm not being super careful w/ error handling due to scope of when we're using this & b/c majority of non-explicit errors will be caught by default case
                    if (stack.Count < 2) {
                        throw new InvalidOperationException(
                            $"RPN {expression} has insufficient operands for operator: {token}");
                    }

                    float a = stack.Pop();
                    float b = stack.Pop();
                    stack.Push(ApplyOperator(token[0], b, a));
                    continue;
                }


                bool success = vars.TryGetValue(token, out float value);
                if (!success) {
                    success = float.TryParse(token, out value);
                }

                // Throw an error if the token is neither a valid operator nor a number/variable
                if (!success) throw new InvalidOperationException($"Invalid token: {token}");
                stack.Push(value);
            }

            if (stack.Count != 1) {
                throw new InvalidOperationException($"{expression} is not a valid RPN expression!");
            }

            return stack.Pop();
        }

        // At some point it may be worth revisiting to add float support, trig, sqrt, etc.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ApplyOperator(char op, float b, float a) {
            // If we want to be silly, we could use bytes here instead >:)
            return op switch {
                '+'             => b + a,
                '-'             => b - a,
                '*'             => b * a,
                '/' when a == 0 => throw new DivideByZeroException("Division by zero."),
                '/'             => b / a,
                '%'             => b % a,
                '^'             => Mathf.Pow(b, a),
                _               => throw new InvalidOperationException($"Unsupported operator: {op}")
            };
        }
        #endregion
    }
    
    // Consider Shunting Yard algorithm for conversions
}