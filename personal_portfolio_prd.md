# Product Requirement Document (PRD)
**Adib Hasan Avik — Personal Portfolio**
* **Version:** 1.0
* **Status:** Draft
* **Product Type:** Personal Portfolio Web Application
* **Technology:** ASP.NET Core MVC
* **Owner:** Adib Hasan Avik

---

## 1. Product Overview

### 1.1 Product Description
A modern, responsive personal portfolio website designed to showcase Adib Hasan Avik's professional identity, software development skills, engineering projects, and technical achievements.
The website will serve as a professional digital presence for recruiters, hiring managers, and potential collaborators to understand the developer's capabilities and explore his work.

### 1.2 Product Vision
Build a professional digital portfolio that demonstrates practical software engineering capabilities through real projects, technical expertise, and a clean, modern user experience.

### 1.3 Professional Identity
* **Name:** Adib Hasan Avik
* **Designation:** Full-Stack .NET Developer | ASP.NET Core Specialist
* **Professional Headline:** Building scalable, secure, and maintainable web applications using modern .NET technologies and software engineering principles.

### 1.4 Primary Objectives
* Establish a strong professional online presence.
* Showcase technical skills and software engineering knowledge.
* Demonstrate practical experience through real-world projects.
* Provide easy access to resume and professional profiles.
* Enable recruiters and visitors to contact the developer.

---

## 2. Target Audience
* Technical Recruiters
* Software Engineering Managers
* Senior Software Developers
* CTOs and Technical Decision-Makers
* Professional Collaborators

---

## 3. Functional Requirements

### FR-01: Hero Section
Display the developer's name, professional designation, introduction, and primary actions.
* **Required elements:**
  * Name: Adib Hasan Avik.
  * Professional designation.
  * Short professional introduction.
  * Professional profile image (if selected).
  * GitHub, LinkedIn, and Email links.
* **Actions:**
  * **Explore Projects:** Smoothly navigate to the projects section.
  * **Download Resume:** Open or download the PDF resume from `/assets/resume.pdf`.

### FR-02: About Section
Present a brief professional introduction covering development interests, engineering approach, and career objectives.
* **Education:**
  * B.Sc. in Computer Science & Engineering.
  * University of Global Village (UGV), Barisal.
* **Professional Training:**
  * DevSkill — Full Stack ASP.NET Core Web Development.
  * Batch 13.

### FR-03: Technical Skills
Display technical competencies using categorized visual badges or icons.
* **Backend & Architecture:** C#, ASP.NET Core, ASP.NET Core MVC, RESTful APIs, Entity Framework Core, LINQ, Clean Architecture, CQRS, Repository Pattern, Unit of Work.
* **Database & Security:** SQL Server, EF Core Migrations, ASP.NET Core Identity, Authentication, Authorization, Claims-Based Authorization.
* **Testing:** NUnit, Moq, AutoMock, Shouldly.
* **DevOps & Tools:** Docker, Docker Compose, Git, GitHub, Visual Studio, VS Code, Postman, Serilog, Autofac, Mailtrap.
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap, Tailwind CSS, jQuery, AJAX.

### FR-04: Featured Projects
Showcase software projects with descriptions, technologies, screenshots, and relevant links.
* **Featured Project:** MaleFashion
* **Category:** Full-Stack E-Commerce Application.
* **Description:** A men's fashion e-commerce web application developed using ASP.NET Core, Clean Architecture, and modern software engineering practices.
* **Live Demo:** https://adibhasan99-001-site1.ftempurl.com
* **GitHub:** https://github.com/Avik-99
* **Technical Highlights:**
  * Clean Architecture and layered separation of concerns.
  * ASP.NET Core Identity and role-based authorization.
  * Entity Framework Core and SQL Server.
  * CQRS and Repository Pattern.
  * Product and inventory management.
  * Unit testing using NUnit, Moq, and Shouldly.
  * Docker-based containerization and deployment.

* **Each project should display:**
  * Project name and overview.
  * Key features.
  * Technology stack.
  * Important technical implementations.
  * Project screenshots.
  * GitHub and live demo links, where available.

### FR-05: Engineering Achievements
Display important technical accomplishments in a simple timeline or card layout.
* **Initial achievements:**
  1. Implemented Docker-based application containerization.
  2. Developed automated unit tests for application handlers.
  3. Customized ASP.NET Core Identity and authorization.
  4. Deployed and tested an ASP.NET Core application in a production hosting environment.

### FR-06: Contact Section
Provide a contact form with the following fields:
* Name
* Email
* Subject
* Message
* **Requirements:**
  * Client-side and server-side validation.
  * Secure form submission.
  * Success and error notifications.
  * Email delivery to `adibhasanavik99@gmail.com`.
  * Anti-spam protection.
* **Direct Contact:**
  * Email: adibhasanavik99@gmail.com
  * GitHub: https://github.com/Avik-99
  * LinkedIn: https://linkedin.com/in/adibhasanavik

### FR-07: Theme Management
* Support dark and light themes.
* Provide a theme toggle.
* Detect system theme preference on first visit.
* Persist user preference using `localStorage`.
* Maintain consistent styling across both themes.

### FR-08: Navigation & Footer
* **Navigation:** Home, About, Skills, Projects, Achievements, Contact.
* **Footer:** Developer identity, Copyright information, Social profile links, Email contact.

---

## 4. Non-Functional Requirements

* **Responsive Design:** Support mobile, tablet, laptop, and desktop screens. Maintain consistent layouts and usability across devices.
* **Performance:** Optimize images, CSS, and JavaScript. Minimize unnecessary dependencies. Ensure fast initial page loading.
* **Security:** Use HTTPS in production. Apply server-side input validation. Use anti-forgery tokens for applicable POST requests. Protect email credentials and application configuration. Implement appropriate spam protection.
* **Accessibility:** Use semantic HTML. Support keyboard navigation. Provide accessible form labels. Maintain sufficient color contrast.
* **Maintainability:** Follow clean coding conventions. Use reusable components and partial views. Maintain separation of concerns. Keep application configuration secure.

---

## 5. Technology Stack

| Component | Technology |
| :--- | :--- |
| **Backend** | ASP.NET Core MVC |
| **Language** | C# |
| **Frontend** | HTML5, CSS3, JavaScript |
| **UI Framework** | Bootstrap 5 / Tailwind CSS |
| **Database** | SQL Server (if required) |
| **Email** | SMTP / Email Service Provider |
| **Logging** | Serilog |
| **Version Control** | Git & GitHub |
| **Containerization**| Docker |
| **Testing** | NUnit, Moq, Shouldly |

---

## 6. UI/UX Requirements
The portfolio should follow a modern, minimalist, premium design direction.
* Clean and professional layout.
* Strong typography and visual hierarchy.
* Consistent spacing and alignment.
* Subtle animations and transitions.
* Carefully selected accent colors.
* Professional project presentation.
* Consistent dark and light themes.
* Mobile-first responsive experience.

The interface should prioritize clarity, readability, and ease of navigation rather than excessive visual effects.

---

## 7. Deployment & Acceptance Criteria
The portfolio is ready for release when:
* All required sections are implemented.
* Navigation and CTA buttons work correctly.
* Resume PDF opens successfully.
* Project links and social profiles are verified.
* Contact form submissions work correctly.
* Dark and light themes function properly.
* Responsive layouts are verified across devices.
* No critical application errors remain.
* The application is successfully deployed using HTTPS.

---
**Document Owner:** Adib Hasan Avik  
**Document Version:** 1.0  
**Status:** Draft  