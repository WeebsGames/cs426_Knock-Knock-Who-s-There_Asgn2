using System.Collections.Generic;

public static class QuestionBank
{
    public static List<QuestionData> GetQuestionSet(int setIndex)
    {
        return setIndex switch
        {
            0 => GetSetA(),
            1 => GetSetB(),
            2 => GetSetC(),
            _ => GetSetA()
        };
    }

    private static List<QuestionData> GetSetA()
    {
        return new List<QuestionData>
        {
            new QuestionData
            {
                questionText = "Which CPU unit performs arithmetic operations?",
                answers = new[] {"Control Unit", "ALU", "Cache", "Register File"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which memory is volatile?",
                answers = new[] {"ROM", "SSD", "RAM", "Flash"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "What does CPU stand for?",
                answers = new[] {"Central Processing Unit", "Computer Power Unit", "Central Program Utility", "Control Processing Utility"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "Which is typically fastest?",
                answers = new[] {"L1 Cache", "RAM", "SSD", "HDD"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "A register is located in:",
                answers = new[] {"Hard Drive", "CPU", "Network Card", "Power Supply"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which bus carries memory addresses?",
                answers = new[] {"Data Bus", "Control Bus", "Address Bus", "I/O Bus"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "Which unit controls instruction flow in CPU?",
                answers = new[] {"ALU", "Control Unit", "Cache", "SSD"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which memory is closest to CPU registers?",
                answers = new[] {"L1 Cache", "RAM", "HDD", "ROM"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "Which of these is permanent storage?",
                answers = new[] {"RAM", "Cache", "SSD", "Register"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "Binary system uses which two digits?",
                answers = new[] {"0 and 1", "1 and 2", "2 and 3", "0 and 9"},
                correctIndex = 0
            }
        };
    }

    private static List<QuestionData> GetSetB()
    {
        return new List<QuestionData>
        {
            new QuestionData
            {
                questionText = "Which component stores instructions temporarily for active programs?",
                answers = new[] {"RAM", "ROM", "GPU VRAM", "CPU Cooler"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "The Control Unit mainly:",
                answers = new[] {"Draws graphics", "Executes arithmetic", "Coordinates CPU operations", "Stores files permanently"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "What is cache memory used for?",
                answers = new[] {"Long-term storage", "Speed up access to frequently used data", "Cooling the CPU", "Internet buffering only"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which architecture term describes 'Fetch, Decode, Execute'?",
                answers = new[] {"Paging", "Instruction Cycle", "Overclocking", "Virtualization"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which memory is non-volatile?",
                answers = new[] {"RAM", "Cache", "ROM", "Register"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "Which unit is most directly responsible for logic comparisons?",
                answers = new[] {"ALU", "PSU", "SSD Controller", "Northbridge"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "Which component executes program instructions?",
                answers = new[] {"Monitor", "CPU", "Keyboard", "Router"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which memory loses data when power is OFF?",
                answers = new[] {"ROM", "Flash", "RAM", "Blu-ray"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "A cache hit means:",
                answers = new[] {"Data found in cache", "Cache is full", "CPU has failed", "RAM is empty"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "The data bus mainly carries:",
                answers = new[] {"Control signals", "Memory addresses", "Actual data values", "Power"},
                correctIndex = 2
            }
        };
    }

    private static List<QuestionData> GetSetC()
    {
        return new List<QuestionData>
        {
            new QuestionData
            {
                questionText = "What does ALU stand for?",
                answers = new[] {"Arithmetic Logic Unit", "Advanced Load Unit", "Array Logic Utility", "Automatic Link Unit"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "Main memory in most systems refers to:",
                answers = new[] {"Cache", "Registers", "RAM", "Blu-ray"},
                correctIndex = 2
            },
            new QuestionData
            {
                questionText = "Which is closest to the CPU core in speed?",
                answers = new[] {"L1 Cache", "DRAM", "NVMe SSD", "USB Drive"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "Which hardware item usually holds BIOS/firmware?",
                answers = new[] {"ROM/Flash chip", "RAM module", "CPU register", "GPU shader core"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "If power is lost, which data is usually lost first?",
                answers = new[] {"Data in RAM", "Data in SSD", "Data in ROM", "Data in DVD"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "Which bus transfers actual values between CPU and memory?",
                answers = new[] {"Address Bus", "Data Bus", "Control Bus", "Clock Bus"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which is a CPU internal fast storage location?",
                answers = new[] {"Register", "SSD", "USB", "GPU Fan"},
                correctIndex = 0
            },
            new QuestionData
            {
                questionText = "What does ROM usually store?",
                answers = new[] {"Temporary game data", "Firmware/startup code", "Mouse input", "Live network traffic"},
                correctIndex = 1
            },
            new QuestionData
            {
                questionText = "Which component is NOT part of basic CPU architecture?",
                answers = new[] {"ALU", "Control Unit", "Registers", "Power Supply"},
                correctIndex = 3
            },
            new QuestionData
            {
                questionText = "Which operation is most likely done by ALU?",
                answers = new[] {"Saving file to disk", "3 + 5", "Loading OS from SSD", "Rendering monitor image"},
                correctIndex = 1
            }
        };
    }
}
