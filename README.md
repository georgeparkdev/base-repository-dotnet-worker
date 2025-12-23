# Base Repository Starter

## Overview

This repository serves as a robust template for creating new project repositories. It comes pre-configured with advanced AI development workflows, including GitHub Copilot integration and the [BMAD Method](https://github.com/bmad-code-org/BMAD-METHOD/), to accelerate your project setup and ensure best practices from day one.

## Features

- **AI-Driven Development**: Built-in support for GitHub Copilot and advanced prompt workflows
- **BMAD Methodology**: Structured approach for project documentation and architecture
- **Prompt Automation**: Ready-to-use prompt files for agents, chat modes, instructions, and more
- **Easy Customization**: Designed to be cloned and adapted for any new repository

## Quick Start

1. **Clone this repository** to start your new project:
   ```sh
   git clone <this-repo-url> <your-new-project>
   cd <your-new-project>
   ```

2. **Install BMAD** (Best Method for Architecture & Documentation):
   - Run the following command to install BMAD globally:
     ```sh
     npm install -g @bmad/method
     ```
   - Or see the [BMAD Method repository](https://github.com/bmad-code-org/BMAD-METHOD/) for alternative installation options and full documentation.

3. **Define Required Documentation**
   - Use BMAD to generate essential docs (Product Requirements, Architecture, etc.) up to the PRD and architecture docs:
     ```sh
     bmad init
     bmad doc create prd
     bmad doc create architecture
     ```
   - For more details, see the [BMAD Method Guide](https://github.com/bmad-code-org/BMAD-METHOD/).

4. **Set Up the AI Workflow**
   - Run each prompt file below in a fresh chat (in the order listed) to fully enable the AI-powered development workflow:

   **a. Collections (run first):**
   ```sh
   #file:suggest-awesome-github-copilot-collections.prompt.md
   ```

   **b. Agents:**
   ```sh
   #file:suggest-awesome-github-copilot-agents.prompt.md
   ```

   **c. Instructions:**
   ```sh
   #file:suggest-awesome-github-copilot-instructions.prompt.md
   ```

   **d. Prompts:**
   ```sh
   #file:suggest-awesome-github-copilot-prompts.prompt.md
   ```

   **e. Chat Modes:**
   ```sh
   #file:suggest-awesome-github-copilot-chatmodes.prompt.md
   ```

   > [!TIP]
   > Run each prompt in a new chat for best results. This ensures a clean context for each setup phase.

---

## Learn More

- **BMAD Method:** [Full Guide & Documentation](https://github.com/bmad-code-org/BMAD-METHOD/)
- **GitHub Copilot:** [Official Documentation](https://docs.github.com/en/copilot)
- **Awesome Copilot Prompts & Instructions:** [awesome-copilot](https://github.com/github/awesome-copilot)

---
