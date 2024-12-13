namespace VulcanOS;

public static class EntryPoint
{
    private static readonly Dictionary<string, Process> ProcTable = new();
    static readonly Dictionary<int, List<int>> FreeBlocksBySize = new();
    private static readonly Queue<Process> toAllocateProcess = new();
    private static int[] _bits = null!;

    private static ConsoleColor[] _memColors =
    [
        ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan,
        ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow,
        ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Magenta
    ];

    private static int currentColorIndex = 0;

    private const int FirstFit = 1;
    private const int NextFit = 2;
    private const int BestFit = 3;
    private const int QuickFit = 4;
    private const int WorstFit = 5;

    private static int _remainingBits;

    private static readonly Dictionary<int, string> AllocationStrategyNames = new()
    {
        { FirstFit, "First Fit" },
        { NextFit, "Next Fit" },
        { BestFit, "Best Fit" },
        { QuickFit, "Quick Fit" },
        { WorstFit, "Worst Fit" }
    };

    private static int _lastFit;

    private static readonly Dictionary<int, Func<Process, bool>> MemoryAllocationStrategyTable = new()
    {
        { FirstFit, FirstFitAllocation },
        { NextFit, NextFitAllocation },
        { BestFit, BestFitAllocation },
        { QuickFit, QuickFitAllocation },
        { WorstFit, WorstFitAllocation },
    };

    public static void Main(string[] args)
    {
        Console.Clear();

        Console.Write("Type the memory size: ");
        int size = int.Parse(Console.ReadLine() ?? "0");

        _bits = new int[size];
        _remainingBits = size;

        for (;;)
        {
            Console.Clear();

            Console.WriteLine("Select an option:");
            Console.WriteLine("\t1. Create process");
            Console.WriteLine("\t2. Run code");
            Console.WriteLine("\t3. Exit");

            int option = int.Parse(Console.ReadLine() ?? "3");

            switch (option)
            {
                case 1:
                    CreateProcess();
                    break;

                case 2:
                    Run();
                    return;

                case 3:
                    return;

                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private static void PrintMemoryState()
    {
        Console.Write("[");
        ConsoleColor[] memoryState = new ConsoleColor[_bits.Length];
        ConsoleColor defaultColor = ConsoleColor.Gray;

        Array.Fill(memoryState, defaultColor);

        foreach (Process process in ProcTable.Values)
        {
            for (int i = process.AllocationIndex; i < process.AllocationIndex + process.BlockSize; i++)
            {
                memoryState[i] = _memColors[process.ColorIndex];
            }
        }

        for (int i = 0; i < _bits.Length; i++)
        {
            Console.ForegroundColor = memoryState[i];
            Console.Write(_bits[i] == 1 ? "\u2593" : "\u2591");
        }

        Console.ResetColor();
        Console.WriteLine("]");
    }

    private static void Run()
    {
        Console.Clear();
        PrintMemoryState();

        DispatchToAllocateProcessQueue();

        for (;;)
        {
            PrintMemoryState();

            Console.WriteLine("Select an option:");
            Console.WriteLine("\t1. Create process");
            Console.WriteLine("\t2. Free process");
            Console.WriteLine("\t3. Exit");

            int option = int.Parse(Console.ReadLine() ?? "1");

            Console.Clear();

            switch (option)
            {
                case 1:
                    CreateProcess();
                    DispatchToAllocateProcessQueue();
                    break;

                case 2:
                    Console.Clear();
                    Console.WriteLine("Current proc table:");
                    Console.WriteLine("Process Name | Block Size");
                    foreach (Process process in ProcTable.Values)
                    {
                        Console.WriteLine($"\t{process.Name} | {process.BlockSize}");
                    }

                    Console.WriteLine("\n");
                    Console.WriteLine("Type the process name: ");
                    string processName = Console.ReadLine() ?? "";
                    if (!ProcTable.TryGetValue(processName, out var value))
                    {
                        Console.WriteLine("Process not found");
                        return;
                    }

                    Console.Clear();
                    FreeProcess(value);
                    PrintMemoryState();
                    break;

                case 3:
                    return;
            }
        }
    }

    private static void DispatchToAllocateProcessQueue()
    {
        foreach (Process process in toAllocateProcess)
        {
            PrintMemoryState();
            Console.WriteLine(
                $"Now trying to allocate {process.BlockSize} bytes of memory for {process.Name} using {AllocationStrategyNames[process.MemoryAllocationStrategy]}."
            );
            AllocateMemoryForProcess(process);
            Console.WriteLine("\n");
        }

        toAllocateProcess.Clear();

        Console.WriteLine("Final memory state:");
        PrintMemoryState();
    }

    private static void FreeProcess(Process process)
    {
        for (int i = process.AllocationIndex; i < process.BlockSize + process.AllocationIndex; i++)
        {
            _bits[i] = 0;
        }

        _remainingBits += process.BlockSize;

        ProcTable.Remove(process.Name);
    }

    private static void AllocateMemoryForProcess(Process process)
    {
        if (process.BlockSize > _remainingBits)
        {
            Console.WriteLine($"No memory available for {process.Name}.");
            if (ProcTable.ContainsKey(process.Name))
            {
                ProcTable.Remove(process.Name);
            }

            return;
        }

        if (!MemoryAllocationStrategyTable[process.MemoryAllocationStrategy](process))
        {
            Console.WriteLine($"No memory available for {process.Name}.");
        }
        else
        {
            Console.WriteLine("Memory allocation succeeded.");
            _remainingBits -= process.BlockSize;
            Console.WriteLine($"Remaining bits: {_remainingBits}");
        }
    }

    private static void Allocate(int start, Process process)
    {
        process.AllocationIndex = start;

        for (int i = start; i < process.BlockSize + start; i++)
        {
            _bits[i] = 1;
        }
    }

    private static bool FirstFitAllocation(Process process)
    {
        int start = 0;
        int current = 0;

        for (;;)
        {
            if (current >= _bits.Length)
            {
                return false;
            }

            if (_bits[current] == 0)
            {
                current++;
            }
            else
            {
                current++;
                start = current;
            }

            if (current - start != process.BlockSize) continue;

            Allocate(start, process);
            _lastFit = current;
            return true;
        }
    }

    private static bool NextFitAllocation(Process process)
    {
        int start = _lastFit;
        int current = _lastFit;
        int wraps = 0;

        for (;;)
        {
            if (current >= _bits.Length)
            {
                if (wraps >= 1) return false;
                wraps++;
                current = 0;
            }

            if (_bits[current] == 0)
            {
                current++;
            }
            else
            {
                current++;
                start = current;
            }

            if (current - start != process.BlockSize) continue;

            Allocate(start, process);
            _lastFit = current;
            return true;
        }
    }

    private static bool BestFitAllocation(Process process)
    {
        int bestStart = -1;
        int bestSize = int.MaxValue;

        int current = 0;
        while (current < _bits.Length)
        {
            if (_bits[current] == 0)
            {
                int start = current;
                while (current < _bits.Length && _bits[current] == 0)
                {
                    current++;
                }

                int size = current - start;
                if (size >= process.BlockSize && size < bestSize)
                {
                    bestStart = start;
                    bestSize = size;
                }
            }
            else
            {
                current++;
            }
        }

        if (bestStart != -1)
        {
            Allocate(bestStart, process);
            return true;
        }

        return false;
    }

    static void UpdateFreeBlocks()
    {
        FreeBlocksBySize.Clear();
        int current = 0;

        while (current < _bits.Length)
        {
            if (_bits[current] == 0)
            {
                int start = current;
                while (current < _bits.Length && _bits[current] == 0)
                {
                    current++;
                }

                int size = current - start;
                if (!FreeBlocksBySize.ContainsKey(size))
                {
                    FreeBlocksBySize[size] = new List<int>();
                }

                FreeBlocksBySize[size].Add(start);
            }
            else
            {
                current++;
            }
        }
    }

    private static bool QuickFitAllocation(Process process)
    {
        UpdateFreeBlocks();

        foreach (var size in FreeBlocksBySize.Keys.OrderBy(size => size))
        {
            if (size >= process.BlockSize)
            {
                int start = FreeBlocksBySize[size][0];
                FreeBlocksBySize[size].RemoveAt(0);
                if (FreeBlocksBySize[size].Count == 0)
                {
                    FreeBlocksBySize.Remove(size);
                }

                Allocate(start, process);
                return true;
            }
        }

        return false;
    }

    private static bool WorstFitAllocation(Process process)
    {
        int worstStart = -1;
        int worstSize = 0;

        int current = 0;
        while (current < _bits.Length)
        {
            if (_bits[current] == 0)
            {
                int start = current;
                while (current < _bits.Length && _bits[current] == 0)
                {
                    current++;
                }

                int size = current - start;
                if (size >= process.BlockSize && size > worstSize)
                {
                    worstStart = start;
                    worstSize = size;
                }
            }
            else
            {
                current++;
            }
        }

        if (worstStart != -1)
        {
            Allocate(worstStart, process);
            return true;
        }

        return false;
    }

    private static void CreateProcess()
    {
        Console.Clear();

        Console.Write("Type the process name: ");
        string name = Console.ReadLine() ?? string.Empty;

        if (ProcTable.ContainsKey(name))
        {
            Console.Error.WriteLine($"Process {name} already exists.");
            Environment.Exit(-1);
        }

        Console.Clear();

        Console.WriteLine("Select the memory allocation strategy: ");
        Console.WriteLine("\t1. First Fit");
        Console.WriteLine("\t2. Next Fit");
        Console.WriteLine("\t3. Best Fit");
        Console.WriteLine("\t4. Quick Fit");
        Console.WriteLine("\t5. Worst Fit");

        int strategy = Math.Clamp(int.Parse(Console.ReadLine() ?? "1"), 1, 5);

        Console.Clear();

        Console.Write("Type the memory size: ");
        int size = int.Parse(Console.ReadLine() ?? "0");

        Process entry = new Process(name, size, strategy);

        toAllocateProcess.Enqueue(entry);
        ProcTable.Add(entry.Name, entry);
        ProcTable[entry.Name].ColorIndex = currentColorIndex++ % _memColors.Length;
    }

    private record Process(string Name, int BlockSize, int MemoryAllocationStrategy)
    {
        public int ColorIndex = 0;
        public int AllocationIndex = 0;

        public override string ToString()
        {
            return $"Name: {Name}, BlockSize: {BlockSize}, MemoryAllocationStrategy: {MemoryAllocationStrategy}";
        }
    }
}