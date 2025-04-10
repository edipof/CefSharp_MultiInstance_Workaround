# CefSharp Multiple Instances Workaround

This repository provides a workaround solution for running multiple instances of the **CefSharp** browser in a **WinForms** application without encountering issues related to cache conflicts. The problem arises when multiple instances of the **same application** try to access and use the **same root cache directory**, leading to unexpected behaviors and conflicts.

## Problem Description

When running multiple instances of the **same CefSharp-based WinForms application**, the following issue occurs:

- **Cache Conflicts**: All instances share the same root cache directory by default. This leads to cache conflicts when each instance tries to read or write data from the same cache location. This can cause unpredictable behavior across instances.
- **Session Issues**: The shared cache might result in one instance affecting the session data or cookies of another instance, leading to inconsistencies.
- **Data Overwrites**: Since all instances are using the same cache, persistent data (like local storage, cookies, and session information) can be overwritten or incorrectly accessed.

## Solution

This project addresses these issues by creating a workaround that ensures each **CefSharp** instance has its own isolated cache and session environment. The solution includes:

1. **Separate Cache Directories**: Each instance of **CefSharp** is configured to use a unique cache directory, preventing multiple instances from sharing the same cache.
2. **Custom Instance Configuration**: Custom settings are applied to each **CefSharp** instance to ensure that cookies, session data, and local storage remain isolated from one another.
3. **Process Isolation**: Each instance of the application runs with its own cache and session state, ensuring that no instance affects another.

## How to Use

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/CefSharp_Multiple_Instances_Workaround.git
   
2. Open the solution in Visual Studio.

3. Build the project.

4. Run the application. Now, when you launch multiple instances of the same application, each instance will use its own cache and session data, avoiding conflicts.
