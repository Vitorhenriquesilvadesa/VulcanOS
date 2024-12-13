namespace VulcanOS.Core
{
    public class Cpu
    {
        private Dictionary<Registers, int> registers = new()
        {
            // General purpose registers
            { Registers.Ax, 0 },
            { Registers.Bx, 0 },
            { Registers.Cx, 0 },
            { Registers.Dx, 0 },

            // Offset data space registers
            { Registers.Si, 0 },
            { Registers.Di, 0 },

            // Stack pointer register
            { Registers.Sp, 0 },

            // Stack frame registers
            { Registers.Bp, 0 },

            // Segment registers
            { Registers.Cs, 0 },
            { Registers.Ds, 0 },
            { Registers.Ss, 0 },
            { Registers.Es, 0 },

            // Instruction pointer register
            { Registers.Ip, 0 },
        };

        public int GetValue(Registers register) => registers[register];

        public void SetValue(Registers register, int value)
        {
            registers[register] = value;
        }

        public void Execute(Instruction instruction)
        {
            Execute(instruction.Code, instruction.Registers);
        }

        private void Execute(InstructionCode instructionCode, params Registers[] operands)
        {
            switch (instructionCode)
            {
                // Arithmetic instructions
                case InstructionCode.Add:
                    if (operands.Length == 2)
                    {
                        byte result = (byte)(GetValue(operands[0]) + GetValue(operands[1]));
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.AddI:
                    if (operands.Length == 2)
                    {
                        int result = (byte)(GetValue(operands[0]) + GetValue(operands[1]));
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Sub:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) - GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.SubI:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) - GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Inc:
                    if (operands.Length == 1)
                    {
                        int result = GetValue(operands[0]) + 1;
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Dec:
                    if (operands.Length == 1)
                    {
                        int result = GetValue(operands[0]) - 1;
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Mul:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) * GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Div:
                    if (operands.Length == 2)
                    {
                        int divisor = GetValue(operands[1]);
                        if (divisor != 0)
                        {
                            int result = GetValue(operands[0]) / divisor;
                            SetValue(operands[0], result);
                        }
                        else
                        {
                            Console.WriteLine("Division by zero");
                        }
                    }

                    break;

                // Bitwise instructions
                case InstructionCode.And:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) & GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Or:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) | GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Xor:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) ^ GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Not:
                    if (operands.Length == 1)
                    {
                        int result = ~GetValue(operands[0]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.AndI:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) & GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.OrI:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) | GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                // Shift instructions
                case InstructionCode.Shl:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) << GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Shr:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) >> GetValue(operands[1]);
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Rol:
                    if (operands.Length == 2)
                    {
                        int result = (GetValue(operands[0]) << GetValue(operands[1])) |
                                      (GetValue(operands[0]) >> (8 - GetValue(operands[1])));
                        SetValue(operands[0], result);
                    }

                    break;

                case InstructionCode.Ror:
                    if (operands.Length == 2)
                    {
                        int result = (GetValue(operands[0]) >> GetValue(operands[1])) |
                                      (GetValue(operands[0]) << (8 - GetValue(operands[1])));
                        SetValue(operands[0], result);
                    }

                    break;

                // Comparison instructions
                case InstructionCode.Cmp:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) - GetValue(operands[1]);
                        SetValue(Registers.Cs, result); // Store result in a condition flag register
                    }

                    break;

                case InstructionCode.CmpI:
                    if (operands.Length == 2)
                    {
                        int result = GetValue(operands[0]) - GetValue(operands[1]);
                        SetValue(Registers.Cs, result); // Store result in a condition flag register
                    }

                    break;

                case InstructionCode.Tst:
                    if (operands.Length == 1)
                    {
                        int result = GetValue(operands[0]) & GetValue(operands[0]);
                        SetValue(Registers.Cs, result); // Store result in a condition flag register
                    }

                    break;

                // Jump instructions
                case InstructionCode.Jmp:
                    // Jump to address (can be a label or memory address)
                    SetValue(Registers.Ip, GetValue(operands[0]));
                    break;

                case InstructionCode.Je:
                    if (GetValue(Registers.Cs) == 0)
                    {
                        SetValue(Registers.Ip, GetValue(operands[0]));
                    }

                    break;

                case InstructionCode.Jne:
                    if (GetValue(Registers.Cs) != 0)
                    {
                        SetValue(Registers.Ip, GetValue(operands[0]));
                    }

                    break;

                case InstructionCode.Jl:
                    if (GetValue(Registers.Cs) < 0)
                    {
                        SetValue(Registers.Ip, GetValue(operands[0]));
                    }

                    break;

                case InstructionCode.Jle:
                    if (GetValue(Registers.Cs) <= 0)
                    {
                        SetValue(Registers.Ip, GetValue(operands[0]));
                    }

                    break;

                case InstructionCode.Jg:
                    if (GetValue(Registers.Cs) > 0)
                    {
                        SetValue(Registers.Ip, GetValue(operands[0]));
                    }

                    break;

                case InstructionCode.Jge:
                    if (GetValue(Registers.Cs) >= 0)
                    {
                        SetValue(Registers.Ip, GetValue(operands[0]));
                    }

                    break;

                // Stack and memory instructions
                case InstructionCode.Call:
                    // Push IP to stack, then jump to address
                    SetValue(Registers.Sp, GetValue(Registers.Sp) - 1); // Decrement stack pobyteer
                    SetValue(Registers.Sp, GetValue(Registers.Ip)); // Save current IP to stack
                    SetValue(Registers.Ip, GetValue(operands[0])); // Jump to function address
                    break;

                case InstructionCode.Ret:
                    // Pop IP from stack and return
                    SetValue(Registers.Ip, GetValue(Registers.Sp));
                    SetValue(Registers.Sp, GetValue(Registers.Sp) + 1); // Increment stack pobyteer
                    break;

                case InstructionCode.Ld:
                    // Load value from memory to register (assuming operands[0] is register, operands[1] is memory address)
                    SetValue(operands[0], GetValue(operands[1]));
                    break;

                case InstructionCode.St:
                    // Store value from register to memory (assuming operands[0] is memory address, operands[1] is register)
                    SetValue(operands[0], GetValue(operands[1]));
                    break;

                case InstructionCode.Lda:
                    // Load value from memory address byteo accumulator register
                    SetValue(Registers.Ax, GetValue(operands[0]));
                    break;

                case InstructionCode.Sta:
                    // Store value from accumulator to memory address
                    SetValue(operands[0], GetValue(Registers.Ax));
                    break;

                case InstructionCode.Push:
                    // Push value onto stack
                    SetValue(Registers.Sp, GetValue(Registers.Sp) - 1);
                    SetValue(operands[0], GetValue(operands[0]));
                    break;

                case InstructionCode.Pop:
                    // Pop value from stack
                    SetValue(operands[0], GetValue(Registers.Sp));
                    SetValue(Registers.Sp, GetValue(Registers.Sp) + 1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(instructionCode), instructionCode, null);
            }
        }
    }
}