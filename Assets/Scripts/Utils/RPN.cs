using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace CMPM.Utils {
    public static class RPN {
        static readonly char[] SUPPORTED_TOKENS = { '+', '-', '*', '/', '%', '^' };
        
        public static int Evaluate(in string expression, in Dictionary<string, int> vars) {
            Stack<int> stack = new();
            foreach (string token in expression.Split(' ')) {
                if (SUPPORTED_TOKENS.Contains(token[0])) {
                    // I'm not being super careful w/ error handling due to scope of when we're using this & b/c majority of non-explicit errors will be caught by default case
                    if (stack.Count < 2) throw new InvalidOperationException($"RPN {expression} has insufficient operands for operator: {token}");
                    
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
    }
    
    // Consider Shunting Yard algorithm for conversions
}