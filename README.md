# NetTask

## Overview

**NetTask** is a robust networking utility designed to streamline and simplify common networking operations. Built with a focus on modularity and efficiency, it offers essential tools for network analysis, IP classification, subnet mask calculation, and MAC address discovery. The project provides a foundation for exploring networking principles while offering practical functionality for real-world use.

## Features

- **IP Classification**: Automatically determines the class of an IP address (A, B, C, D, E) and provides relevant details such as default subnet masks.
- **Subnet Mask Calculation**: Dynamically calculates subnet masks based on IP classes or custom CIDR notation.
- **MAC Address Discovery**: Performs ARP (Address Resolution Protocol) requests to retrieve MAC addresses of devices within the local network.
- **Network Analysis**: Offers tools to analyze network properties and provide meaningful insights for debugging and configuration.
- **Dynamic Execution**: Handles real-time updates and exceptions gracefully for seamless operation.

## Architecture

NetTask is built with a modular design, ensuring scalability and ease of maintenance. Its core components include:

- **IP Address Utilities**:
  - Functions for identifying IP classes and calculating default subnet masks.
  - Logical separation of IP operations for better maintainability.

- **Subnetting Logic**:
  - Support for dynamic CIDR-based subnet calculations.
  - Designed to handle edge cases and invalid inputs effectively.

- **ARP Handler**:
  - Sends ARP requests and captures responses to discover active devices on the local network.
  - Employs low-level network operations to provide accurate results.

- **Core Execution**:
  - Centralized logic for managing network tasks.
  - Ensures modularity by separating UI, networking, and execution layers.

## Contribution

Contributions are welcome! If you want to enhance the project, add features, or report a bug, feel free to open an issue or submit a pull request. 

### Contribution Guidelines:

1. Fork the repository.
2. Create a new feature branch.
3. Make your changes and document them clearly.
4. Submit a pull request with a detailed explanation of your updates.

Let's work together to make **NetTask** an even more powerful tool for networking enthusiasts and professionals!
