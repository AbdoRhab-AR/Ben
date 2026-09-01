# ENTERPRISE SYSTEM ARCHITECTURE SPECIFICATION

## KITCHEN OPERATIONAL & SALES SYSTEM (KOSS)
### An Integrated, End-to-End Enterprise Resource Planning (ERP) Workflow Specification for Libyan Kitchen Design and Manufacturing Operations

**Author:** Malik (System Lead Architect & Developer)  
**Prepared For:** Company Executive Board (Ali, Ezz, Ahmad)  
**Date:** August 31, 2026  
**Target Market:** Libyan Kitchen & Furniture Manufacturing Sector  
**Implementation Scope:** Sales, Finance, CAD Design, Nesting Optimization, and Factory Handoff  
**Document Version:** 1.0 (Official Release)  

---

### SYSTEM INTEGRATION NOTICE
> **Important:** This system is designed to completely replace legacy paper records and manual Microsoft Excel files, establishing a unified, multi-user real-time operational database that links the Sales Showroom directly to the Production Factory Floor.

---

## TABLE OF CONTENTS
1. [Executive Summary & Lifecycle Implementation](#1-executive-summary--lifecycle-implementation)
2. [Phase 1: Customer Intake & Lead Management](#2-phase-1-customer-intake--lead-management)
3. [Phase 2: Technical Field Survey & Tiered Design Fees](#3-phase-2-technical-field-survey--tiered-design-fees)
4. [Phase 3: 3D CAD Design & AI-Assisted Prototyping](#4-phase-3-3d-cad-design--ai-assisted-prototyping)
5. [Phase 4: Financial Controls, Deposit Policies, & Dynamic Pricing](#5-phase-4-financial-controls-deposit-policies--dynamic-pricing)
6. [Phase 5: Technical Detailing, Nesting, & Supply Chain BOM](#6-phase-5-technical-detailing-nesting--supply-chain-bom)
7. [Phase 6: Factory Manufacturing, Field Installation, & Handover](#7-phase-6-factory-manufacturing-field-installation--handover)
8. [Phase 7: Human Factors, Change Management, & Accountability](#8-phase-7-human-factors-change-management--accountability)

---

## 1. EXECUTIVE SUMMARY & LIFECYCLE IMPLEMENTATION
The Kitchen Operational and Sales System (KOSS) is a custom-built enterprise resource planning (ERP) platform. Its primary objective is to eliminate the severe operational bottlenecks, design delays, and financial inaccuracies associated with previous manual processes and fragmented Excel sheets. The implementation strategy represents a cohesive system where every stage is programmatically dependent on preceding transactions.

### Implementation Schedule
The entire software development lifecycle is strictly defined as a **6-week program**. To guarantee seamless field adoption and eliminate software bugs, the deployment schedule is structured as follows:

*   **Weeks 1 to 2 (Analysis Phase):** In-depth business process mapping, data analysis, database schema structuring, and stakeholder alignment meetings.
*   **Weeks 3 to 4 (Development Phase):** Full-stack software engineering including database building, backend API programming, and frontend user interface development.
*   **Weeks 5 to 6 (Beta Testing Phase):** Delivery of an **80% functional prototype** to the company showroom and factory for real-world testing. This active testing phase allows staff to run concurrent operations and provide critical feedback before final launch.

---

## 2. PHASE 1: CUSTOMER INTAKE & LEAD MANAGEMENT
Every customer engagement must originate within the KOSS Sales Showroom interface. This represents the entry point of the entire business workflow. The sales personnel or reception desk is responsible for logging the client's initial contact details and initiating the sales funnel.

### Client Classification Status
Upon entry, the customer is assigned a system status representing their stage in the sales cycle. The initial status is set to either:
*   **'Interested' (مهتم)**
*   **'Not Interested' (غير مهتم)**

This primary classification dictates whether a technical file is generated and whether resources are assigned to the lead.

---

## 3. PHASE 2: TECHNICAL FIELD SURVEY & TIERED DESIGN FEES
Once a client is classified as **'Interested'**, the system triggers the Technical Field Survey phase. This represents a critical pivot point where physical metrics are captured to drive the subsequent design and manufacturing phases.

### The Measurement Process
A company field technician is scheduled via the system calendar. The technician conducts an on-site physical measurement, capturing precise dimensions of walls, plumbing inlets, electrical outlets, windows, and ceiling heights. Once the data is entered, the contract phase in the KOSS Dashboard transitions automatically to **'Measured' (تم القياس)**.

### Tiered Design Fee Policy
To filter out low-intent inquiries and compensate for technical drafting labor, the company implements a non-refundable, structured **Design Fee Policy**. This fee is paid upfront and is later amortized or deducted from the final kitchen invoice upon contract signature. The pricing model is structured as a stepped scale to incentivize multi-unit designs (e.g. kitchen, dressing room, laundry room):

| Number of Designed Units | Base Rate | Total Cost | System Action & Document Output |
| :--- | :--- | :--- | :--- |
| **1 Unit** (e.g., Main Kitchen) | 300 LYD | 300 LYD | System prints standard receipt with official company watermark. |
| **2 Units** (Kitchen + Laundry) | 300 LYD | 600 LYD | Generates dual-unit system file and schedules site surveyor. |
| **4 Units** (Full Interior Suite) | Discounted | 1,200 LYD | Applies multi-unit promotion; saves 300 LYD compared to standard rates. |
| **5+ Units** | Dynamic Escalation | Negotiated | Requires manager clearance; registers design file in active queue. |

---

## 4. PHASE 3: 3D CAD DESIGN & AI-ASSISTED PROTOTYPING
Once design fees are paid, the customer contract transitions to the **'Designed' (تم التصميم)** status. Designers are assigned the file to generate full visual renderings of the custom layout.

### The CAD Design Bottleneck
Historically, drafting a custom 3D kitchen layout took **3 to 4 working days** per client. Under heavy showroom traffic, this created massive operational backlogs. This operational bottleneck is resolved by utilizing **AI-Assisted Design Tools** within the showroom workflow.

> ### 💡 AI INTEGRATION VALUE
> **AI Design Simulation:** By uploading basic dimensions and structural images of the kitchen space, premium AI-assisted render engines generate photorealistic design alternatives in **under 10 minutes**. This enables immediate client visualization in-showroom, greatly accelerating the sales and quoting process.

---

## 5. PHASE 4: FINANCIAL CONTROLS, DEPOSIT POLICIES, & DYNAMIC PRICING
The financial department utilizes KOSS to enforce rigid fiscal disciplines. No production commands can be issued to the factory floor without satisfying strict pre-payment thresholds.

### The 70% Deposit Rule
Company policy mandates that a **minimum deposit of 70%** (or 60% in specific promotional seasons) must be paid for any item before it is authorized for manufacturing. If a client orders multiple rooms, the deposit must cover the 70% threshold of each specific item independently.

### Multi-Item Account Separation Scenario
When a single client places orders for multiple, distinct units (e.g. a Kitchen, Bedroom, and Dressing Room) but pays a single lump-sum deposit that is less than 70% of the aggregate total, the system programmatically routes the money. 

The funds are directed to satisfy the **70% threshold of the most urgent unit (typically the Kitchen)** to kickstart its production immediately. The other units are flagged as **'Suspended/On Hold' (موقوفة)** in the database.

#### Allocation Breakdown Example:
*   **Total Customer Budget Paid:** 40,000 LYD

| Unit Profile | Estimated Value | 70% Req. | Allocated (From 40k Deposit) | Manufacturing Status |
| :--- | :--- | :--- | :--- | :--- |
| **Main Kitchen** (Urgent) | 40,000 LYD | 28,000 LYD | **28,000 LYD** (Allocated first) | **ACTIVE** - Released to factory floor. |
| **Bedroom Suite** | 20,000 LYD | 14,000 LYD | **12,000 LYD** (Partial remaining) | **SUSPENDED** - Held in system queue. |
| **Dressing Room** | 20,000 LYD | 14,000 LYD | **0 LYD** (No allocation) | **SUSPENDED** - Held in system queue. |
| **TOTAL CONTRACT** | **80,000 LYD** | **56,000 LYD** | **40,000 LYD** (Total Paid) | **PARTIAL RELEASE STATUS** |

### Payment Receipt Serialization
All payments trigger the automatic generation of a **serialized, unique receipt** from the database. The system prevents duplicate receipt numbers across different customers. The financial ledger is directly tied to the unified contract transaction ID, enabling a complete audit trail for accountants.

### Dynamic Price Management & Market Volatility
The Libyan raw materials market experiences constant fluctuations in the cost of imported wood sheets (MDF, laminate) and hardware accessories. To prevent selling at a loss, the system incorporates a **Dynamic Pricing Override**. 

The finance department has the sole authority to adjust the **'Price per Meter'** in the centralized database daily. Any showroom salesperson opening a new invoice receives an instant system alert notifying them of updated pricing, ensuring accurate and profitable quoting.

---

## 6. PHASE 5: TECHNICAL DETAILING, NESTING, & SUPPLY CHAIN BOM
Once the financial deposit is cleared, the contract enters the **'Under Production' (تم التنفيذ)** phase. This bridges the showroom design to the mechanical execution on the factory floor.

### The 3D to 2D CAD Conversion
The technical drafting department retrieves the approved 3D model from the system and decomposes it into highly detailed 2D fabrication drawings. These drawings depict every cabinet panel, carcass box, drawer runner, and door face with millimeter precision.

### Wood Sheet Nesting & Material Calculations
The technical supervisor uses KOSS to calculate the exact volume of wood required. This is based on standard imported sheets in the market, which measure exactly **2.80m x 1.22m** (yielding a gross surface area of **3.416 square meters** per sheet). 

The system uses nested layout algorithms to arrange cabinet parts on the physical sheets, calculating the exact yield and preventing costly scrap and offcut waste.

### Bill of Materials (BOM) & Purchase Orders (PO)
The decomposed drawing is mapped against a centralized database containing **104 standardized item codes** (representing various wood finishes, drawer slides, hinges, screws, and accessories). The system automatically compiles these into a structured **Purchase Order (PO)** or **Bill of Materials (BOM)** linked to the client's file. This eliminates manual inventory guesswork.

> ### 📦 SUPPLY CHAIN INTEGRATION
> **Procurement & Factory Loop:** Once the BOM is compiled, KOSS transmits the PO directly to the Warehouse and Accounting departments. The warehouse issues a formal material release ticket, and the accounting department registers the raw material deduction. The optimized cut list is then routed electronically to the factory CNC machinery, preventing manual entry errors.

---

## 7. PHASE 6: FACTORY MANUFACTURING, FIELD INSTALLATION, & HANDOVER
With materials dispatched, physical manufacturing begins in the production facility.

### Manufacturing & Quality Control
Factory floor managers track parts across edge-banding, boring, assembly, and packaging. Upon completion, the project status is updated in the dashboard to **'Manufactured/Executed'**.

### Field Installation & Technical Compensation
Once delivered to the client's residence, the field team begins installation. The system logs the field team's assignment and tracks the physical completion. Once fully installed, the technician updates the status to **'Installed' (تم التركيب)**. 

This action triggers the calculation of the installers' and technicians' compensation, which is dynamically calculated based on the linear meters executed and specific labor items on the contract.

### Final Commissioning & Handover
The system transition to the final stage **'Commissioned/Completed' (تم التشطيب)** represents the formal project closure. This occurs after a field quality audit, client signature of completion, and the final payment of the remaining 30% contract balance, closing the financial ledger.

---

## 8. PHASE 7: HUMAN FACTORS, CHANGE MANAGEMENT, & ACCOUNTABILITY
The primary diagnostic of previous enterprise software failures in the company was not technical or programmatic deficiency, but rather staff resistance and poor change management. Designers, surveyors, and field staff viewed system data entry as an unpaid, time-consuming administrative burden that detracted from their core artistic or technical focus.

To guarantee 100% compliance and active use of the KOSS system, management enforces a structured **'Carrot and Stick' (العصا والجزرة)** policy programmatically built into the operational workflow.

### The Carrot: The Designer and Sales Logging Incentives
To encourage diligent data logging, the system is directly linked to the payroll system. Designers receive a **direct cash bonus** added to their monthly salary for every complete design file, invoice, and bill of materials they successfully log and commit to the KOSS database. This transforms the system from an administrative chore into a direct source of personal revenue, ensuring proactive compliance.

### The Stick: Absolute Technical and Financial Accountability
In exchange for financial incentives, staff assume absolute technical and financial responsibility for their input data. If a designer or field surveyor enters incorrect measurements, skips items on the BOM, or logs faulty pricing in the invoice, they are **legally and financially liable** for the cost of the wasted materials, replacement parts, and corrective labor. This double-layer accountability ensures that all entries in the ERP system are precise and meticulously verified.

#### Operational Staff Role-Responsibility Matrix:

| Operational Staff Role | ERP Action & Data Entry Duty | Incentive Reward (Carrot) | Financial Liability Risk (Stick) |
| :--- | :--- | :--- | :--- |
| **Sales Showroom Staff** | Log client profile; set 'Interested' status; enter client contact data. | Standard showroom performance commission. | Loss of commission if contact data or lead logging is unverified. |
| **Interior Designer** | Upload CAD designs; map 3D layout to 104 database items; compile BOM. | **Cash bonus** per successfully logged design and invoice. | **Full financial liability** for material recut costs and errors in BOM selection. |
| **Field Surveyor / Technician** | Conduct on-site measurements; upload dimensions; verify tolerances. | **Bonus compensation** per completed survey. | **Full legal and financial liability** for fabrication remakes due to wrong dimensions. |
| **Showroom & Plant Managers** | Monitor dashboard KPIs; approve POs; resolve schedule alerts. | Managerial performance bonus based on project cycle speed. | Salary deductions if client files remain stalled in dashboard without reason. |

### Post-Launch Training & Operational Transition
To ease the transition, the system developer (Malik) will conduct a structured **4-week hands-on training program** for all showroom and factory personnel immediately following system handoff. 

During this month-long training window, staff will run concurrent operations to build confidence, learn to resolve minor errors, and fully adapt their daily workflow to the KOSS ERP environment. Following this, the manual Excel sheets will be permanently retired, and KOSS will remain the company's sole, authoritative system of record.
