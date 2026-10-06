# BOOSTLAB Windows

Modern Windows client for BOOSTLAB.

## Technology baseline

- C# on .NET 10.
- WinUI 3 via Windows App SDK 2.5.1.
- Central Package Management.
- Nullable reference types.
- Latest recommended .NET analyzers.
- Warnings treated as errors.
- x64 and ARM64 targets.

The previous WPF shell has been removed from the architecture. Windows-specific UI is built on the current Windows App SDK stack instead.

## Planned responsibilities

- Application/process selection.
- Tunnel lifecycle.
- Gateway discovery and automatic route selection.
- Ping / jitter / packet-loss display.
- WireGuard data path.
- Diagnostics and safe fallback when a gateway is unavailable.

The Windows client will reuse BOOSTLAB network concepts and protocol contracts, but not Android-specific implementation code.
