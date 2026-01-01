# X64Dbg MCP Server Plugin

A Model Context Protocol (MCP) server plugin for [x64dbg](https://github.com/x64dbg/x64dbg) that enables AI-assisted reverse engineering. Built with C# on .NET Framework—no ASP.NET Core required.

This plugin creates a lightweight HTTP bridge between MCP clients and the x64dbg debugger, allowing LLM-powered tools to interactively inspect memory, disassemble code, query registers, manipulate labels/comments, and more—all remotely and programmatically.

![Plugin Screenshot](https://github.com/user-attachments/assets/4b3c3a02-edc0-48e2-93eb-a8c1727b5017)

## Features

- ✅ Self-hosted HTTP server (no ASP.NET Core dependency)
- ✅ Lightweight, zero-dependency deployment
- ✅ Modular command system with parameter mapping
- ✅ Direct access to registers, memory, threads, and disassembly
- ✅ Bidirectional AI/LLM command support
- ✅ Hot-reload plugins without restarting x64dbg
- ✅ Expression function and menu extension support

---

## MCP Client Configuration

### Cursor

Add the following to your Cursor MCP configuration:

```json
{
  "mcpServers": {
    "x64Dbg MCP Server": {
      "url": "http://127.0.0.1:50300/sse"
    }
  }
}
```

![Cursor Configuration](https://github.com/user-attachments/assets/22414a30-d41e-4c3d-9b4f-f168f0498736)

![Cursor Demo](https://github.com/user-attachments/assets/53ba58e6-c97c-4c31-b57c-832951244951)

### Claude Desktop

> **Note:** Claude Desktop requires the [MCPProxy STDIO↔SSE Bridge](https://github.com/AgentSmithers/MCPProxy-STDIO-to-SSE/tree/master).

Add the following to your Claude Desktop configuration:

```json
{
  "mcpServers": {
    "x64Dbg": {
      "command": "C:\\MCPProxy-STDIO-to-SSE.exe",
      "args": ["http://localhost:50300"]
    }
  }
}
```

![Claude Desktop Configuration](https://github.com/user-attachments/assets/0b089015-2270-4b39-ae23-42ce4322ba75)

![Claude Desktop Demo](https://github.com/user-attachments/assets/3ef4cb69-0640-4ea0-b313-d007cdb003a8)

### Windsurf

> **Note:** Windsurf requires the [MCPProxy STDIO↔SSE Bridge](https://github.com/AgentSmithers/MCPProxy-STDIO-to-SSE/tree/master).
>
> **Known Issue:** Direct SSE connections may experience "context deadline exceeded" timeout errors.

Add the following to your Windsurf configuration:

```json
{
  "mcpServers": {
    "x64Dbg STDIO-SSE Bridge": {
      "command": "C:\\MCPProxy-STDIO-to-SSE.exe",
      "args": ["http://localhost:50300"]
    }
  }
}
```

![Windsurf Demo](https://github.com/user-attachments/assets/df900c88-2291-47af-9789-1b17ff51cfa9)

---

## Sample MCP Client

Need a client to get started? Download the sample client:

📦 [mcp-csharp-sdk-client.zip](https://github.com/user-attachments/files/19697365/mcp-csharp-sdk-client.zip)

**Setup Instructions:**

1. Open the project in Visual Studio
2. Edit `Program.cs` line 590: Enter your Google Cloud API Gemini key
3. Edit `Program.cs` line 615: Set your MCP server IP (e.g., `http://192.168.x.x:50300/sse`)
4. Open x64dbg, then start the MCP server manually via the Plugins menu before running the client
5. (Optional) To interact manually instead of using AI, uncomment line 634 and comment out line 635
6. Run the client—the AI will execute the prompt on line 434

![Sample Client](https://github.com/user-attachments/assets/ebf2ad81-0672-4ceb-be6e-a44c625cd6d0)

**Latest Version:** [mcp-csharp-sdk-client on GitHub](https://github.com/AgentSmithers/mcp-csharp-sdk-client/)

---

## Sample Conversations

### AI-Assisted Module Analysis
The AI loads a file, counts internal modules, and labels important functions:
- [View Sample 1](https://github.com/AgentSmithers/x64DbgMCPServer/blob/master/Sample1)

### Speed Hack Identification
Single-shot speed hack detection:
- [View Sample 2](https://github.com/AgentSmithers/x64DbgMCPServer/blob/master/Sample2)

---

## Installation

### Prerequisites

- **Visual Studio Build Tools** (2019 v16.7 or later)
- **.NET Framework 4.7.2 SDK**
- **[3F/DllExport](https://github.com/3F/DllExport)**

> **For x86/32-bit support:** You must install .NET Framework 2.0 and 3.5 via "Turn Windows features on or off" (`appwiz.cpl` → Add/Remove Windows Components).

### Building from Source

1. **Clone the repository:**
   ```bash
   git clone https://github.com/AgentSmithers/x64DbgMCPServer
   ```

2. **Download and run DllExport:**
   - Download [DllExport.bat](https://github.com/3F/DllExport/releases/download/1.8/DllExport.bat)
   - Place it in the root folder (where the `.sln` file is located)
   - Run `DllExport.bat`

3. **Configure DllExport:**
   - Check the `Installed` checkbox
   - Set Namespace to `System.Runtime.InteropServices`
   - Select target platform (`x64` or `x86`)
   - Click **Apply**

   <img width="518" alt="DllExport Configuration" src="https://github.com/user-attachments/assets/5148316e-37fe-48d4-baec-73fb2ef1d3ed" />

4. **Build the solution:**
   - Open the `.sln` file in Visual Studio
   - Build the solution

   > **Build Error?** If you encounter errors, try cleaning and rebuilding `DotNetPlugin.Stub` first.
   >
   > <img width="998" alt="Build Error" src="https://github.com/user-attachments/assets/a4bd8b06-3b35-4e3d-bdea-d7f8627178b3" />

### Installing the Plugin

1. **Rename the output file:**
   - Locate `x64DbgMCPServer.dll` in `x64DbgMCPServer\bin\x64\Debug`
   - Rename it to `x64DbgMCPServer.dp64` (or `.dp32` for 32-bit)

2. **Copy to x64dbg plugins folder:**
   - Create the folder if it doesn't exist: `x64dbg\release\x64\plugins\x64DbgMCPServer`
   - Copy all files from `bin\x64\Debug` to this folder

   ![Plugin Installation](https://github.com/user-attachments/assets/8511452e-b65c-4bc8-83ff-885c384d0bbe)

   <img width="880" alt="Installed Plugin" src="https://github.com/user-attachments/assets/05994544-1b00-4b2d-9998-bf61c72b1425" />

3. **Unblock downloaded files:**
   ```powershell
   Unblock-File *
   ```
   > **Important:** Windows may block downloaded DLLs, preventing .NET Framework from loading them.

---

## Usage

### Starting the Server

1. Launch x64dbg
2. Go to **Plugins** → **Start MCP Server**
3. Connect with your preferred MCP client on port `50300` via SSE

![Server Loaded](https://github.com/user-attachments/assets/02eb35d8-8584-46de-83c6-b535d23976b9)

### Available Commands

The following MCP tools are available for AI-assisted debugging and reverse engineering:

#### Core Debugger Commands

| Command | Description | Example |
|---------|-------------|---------|
| `ExecuteDebuggerCommand` | Execute any x64dbg command synchronously | `command=init c:\Path\To\Program.exe` |
| `ExecuteDebuggerCommandWithVar` | Execute a command and return a debugger variable | `command=init notepad.exe, resultVar=$pid, pollMs=100, pollTimeoutMs=5000` |
| `ExecuteDebuggerCommandWithOutput` | Execute a command and capture log output | `command="bplist"` |
| `ListDebuggerCommands` | List available debugger commands by category | `subject=gui` (options: debugcontrol, gui, search, threadcontrol) |
| `DbgValFromString` | Get a debugger variable/expression value | `value=$pid` |

#### Memory Operations (Debug Session Required)

| Command | Description | Example |
|---------|-------------|---------|
| `ReadDismAtAddress` | Disassemble instructions at an address | `address=0x12345678, byteCount=100` |
| `WriteMemToAddress` | Write bytes to a memory address | `address=0x12345678, byteString=0F FF 90` |

#### Labels and Comments (Debug Session Required)

| Command | Description | Example |
|---------|-------------|---------|
| `GetLabel` | Get the label at a specific address | `addressStr=0x12345678` |
| `CommentOrLabelAtAddress` | Add a label or comment at an address | `address=0x12345678, value=MyFunction, mode=Label` |

#### Process Information (Debug Session Required)

| Command | Description | Example |
|---------|-------------|---------|
| `GetAllRegisters` | Get all CPU register values | — |
| `GetAllActiveThreads` | List all active threads with details | — |
| `GetAllModulesFromMemMap` | List all loaded modules from memory map | — |
| `GetCallStack` | Get the current call stack | `maxFrames=32` |

#### Breakpoints (Debug Session Required)

| Command | Description | Example |
|---------|-------------|---------|
| `GetBreakpointInfo` | Get breakpoint information using safe methods | — |

#### File Operations (Debug Session Required)

| Command | Description | Example |
|---------|-------------|---------|
| `DumpModuleToFile` | Dump module info and disassembly to a file | `pfilepath=C:\Output.txt` |

> **Note:** Commands marked "Debug Session Required" only work when x64dbg is actively debugging a process.

#### Advanced Commands

**`ExecuteDebuggerCommandWithVar`** — Run a command and read a debugger variable:
```
ExecuteDebuggerCommandWithVar command="init notepad.exe" resultVar=$pid pollMs=100 pollTimeoutMs=5000
```
Returns the value of `$pid` (e.g., `0x1234`) after initialization.

**`ExecuteDebuggerCommandWithOutput`** — Run a command and capture log output:
```
ExecuteDebuggerCommandWithOutput command="bplist"
```
Returns the text output produced by the command.

![Command Examples](https://github.com/user-attachments/assets/f954feab-4518-4368-8b0a-d6ec07212122)
![Command Results](https://github.com/user-attachments/assets/2952e4eb-76ef-460c-9124-0e3c1167fa3d)

---

## Troubleshooting

### "Access is denied" when starting MCP server

If you see `Failed to start MCP server: Access is denied` in the x64dbg logs (Alt+L), Windows requires special permissions to listen on HTTP URLs.

**Option 1: Run as Administrator (Quick fix)**
- Right-click `x64dbg.exe` → **Run as administrator**

**Option 2: Grant URL permissions (Recommended)**

Run these commands in an elevated PowerShell or Command Prompt:
```cmd
netsh http add urlacl url=http://+:50300/sse/ user=Everyone
netsh http add urlacl url=http://+:50300/message/ user=Everyone
```

After running these commands, x64dbg can be started normally.

### Plugin fails to load / x64dbg crashes on startup

Ensure downloaded DLLs are not blocked by Windows:
```powershell
Get-ChildItem -Path "path\to\plugins" -Recurse | Unblock-File
```

Blocked files prevent .NET Framework from loading assemblies for security reasons.

---

## Development

### Debugging Setup

The `DotNetPlugin.Impl` project includes post-build commands for rapid iteration. Update the paths to match your x64dbg installation:

```
xcopy /Y /I "$(TargetDir)*.*" "C:\path\to\x64dbg\plugins\x64DbgMCPServer"
C:\path\to\x64dbg\x64dbg.exe
```

This automatically copies the plugin and launches x64dbg after each build.

### Project Structure

- **DotNetPlugin.Stub** — Native entry point for x64dbg
- **DotNetPlugin.Impl** — Main plugin implementation
- **DotNetPlugin.RemotingHelper** — Assembly remoting support

---

## How It Works

The MCP server runs an `HttpListener` that routes incoming requests to C# methods marked with the `[Command]` attribute. These methods can perform any operation (memory reads, disassembly, breakpoints, etc.) and return structured data to MCP clients.

```
MCP Client  →  HTTP Request  →  HttpListener  →  [Command] Method  →  x64dbg API
     ↑                                                                      |
     └──────────────────────  JSON Response  ←──────────────────────────────┘
```

---

## Known Issues

| Issue | Status |
|-------|--------|
| `ExecuteDebuggerCommand` always returns `true` (indicates command was dispatched, not result) | Fix implemented, needs verification |
| Default configuration listens on all IPs (`+`), requiring admin privileges | See [Troubleshooting](#troubleshooting) for workarounds |
| Some commands are not fully implemented | Active development |

---

## Acknowledgments

⚡ Built with the help of [DotNetPluginCS](https://github.com/mrexodia/DotNetPluginCS) by Adams85.

This project represents ~20 hours of focused development, MCP protocol review, and iteration to create a self-contained HTTP MCP server plugin for x64dbg—no Kestrel, no ASP.NET, just raw `HttpListener` powering reverse engineering automation.

---

## Contributing

I'm actively improving this codebase as part of my exploration into AI-assisted analysis and security automation. If you'd like to:

- Create your own integration
- Extend this plugin
- Discuss potential use cases

Feel free to reach out via the repository or my profile. I'm eager to collaborate with others exploring this space.

💻 **Let's reverse engineer smarter, not harder.**

🎉 Cheers!

🌐 [ControllingTheInter.net](https://ControllingTheInter.net)
