namespace VulcanOS.Core;

public record Instruction(InstructionCode Code, params Registers[] Registers)
{
}