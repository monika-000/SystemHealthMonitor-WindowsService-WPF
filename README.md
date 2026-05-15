# System Health Monitor

A comprehensive Windows Service & WPF application designed for continuous system‑health monitoring, real‑time alerting, and extensible metric analysis. The platform combines a multi‑component architecture, robust communication patterns, and automated deployment pipelines to deliver a reliable and maintainable monitoring solution.
## ✨ Features
### Core Architecture
* Windows Service for background metric collection at configurable intervals
* WPF desktop application for interactive monitoring, configuration, and visualisation
* Dual‑mode operation supporting both a full desktop UI and a lightweight tray application for silent background monitoring
*  Named Pipes communication enabling fast, secure interprocess messaging between the service and the WPF app
*  MVVM architecture ensuring clean separation of UI, logic, and data layers

## 📊 System Metrics Monitored
The platform tracks key system‑level metrics with an extensible monitoring design:
* CPU usage
* Disk I/O
* Free disk space
* Memory usage
* Network I/O
* Packet loss
Additional metrics can be added through a modular extension pattern

## 🔔 Alerts & Notifications
* Windows Toast Notifications for real‑time alerts when thresholds are exceeded
*  Configurable thresholds and polling intervals
*   Ability to view, filter, and analyse results, including identifying worst‑performing metrics

## 🧩 Logging & Maintenance

* Comprehensive logging system capturing service activity, metric results, and application events
* Automated cleanup logic for removing old logs and purging outdated metric results to maintain performance and storage efficiency

## 🚀 Installation

Download the latest installer from the **Releases** page:

👉 [https://github.com/<your-repo>/releases/latest](https://github.com/monika-000/SystemHealthMonitor-WindowsService-WPF/releases)

Run the `SystemHealthMonitor.BundleInstaller.exe` to install:
- .NET runtime
- Windows Service
- WPF desktop

## 🧱 Technology Stack
* .NET 10+
* WPF (MVVM)
* Windows Service
* Named Pipes IPC
* WiX Toolset (Burn)
*  GitHub Actions
*  Windows Toast Notifications
*  Serilog
