---
name: Terra Management System
colors:
  surface: '#fcf9f8'
  surface-dim: '#dcd9d9'
  surface-bright: '#fcf9f8'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f6f3f2'
  surface-container: '#f0eded'
  surface-container-high: '#eae7e7'
  surface-container-highest: '#e5e2e1'
  on-surface: '#1c1b1b'
  on-surface-variant: '#52443c'
  inverse-surface: '#313030'
  inverse-on-surface: '#f3f0ef'
  outline: '#84746a'
  outline-variant: '#d7c3b8'
  surface-tint: '#86522c'
  primary: '#613310'
  on-primary: '#ffffff'
  primary-container: '#7d4a25'
  on-primary-container: '#ffbf95'
  inverse-primary: '#fdb789'
  secondary: '#994600'
  on-secondary: '#ffffff'
  secondary-container: '#ff8d41'
  on-secondary-container: '#6a2e00'
  tertiary: '#3f3f3d'
  on-tertiary: '#ffffff'
  tertiary-container: '#575654'
  on-tertiary-container: '#cecbc9'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdcc7'
  primary-fixed-dim: '#fdb789'
  on-primary-fixed: '#311300'
  on-primary-fixed-variant: '#6a3b17'
  secondary-fixed: '#ffdbc8'
  secondary-fixed-dim: '#ffb68b'
  on-secondary-fixed: '#321200'
  on-secondary-fixed-variant: '#753400'
  tertiary-fixed: '#e5e2df'
  tertiary-fixed-dim: '#c8c6c3'
  on-tertiary-fixed: '#1c1c1a'
  on-tertiary-fixed-variant: '#474745'
  background: '#fcf9f8'
  on-background: '#1c1b1b'
  surface-variant: '#e5e2e1'
typography:
  display-lg:
    fontFamily: Be Vietnam Pro
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Be Vietnam Pro
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Be Vietnam Pro
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  headline-md:
    fontFamily: Be Vietnam Pro
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  title-lg:
    fontFamily: Be Vietnam Pro
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Be Vietnam Pro
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-md:
    fontFamily: Be Vietnam Pro
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-md:
    fontFamily: Be Vietnam Pro
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 8px
  container-padding: 32px
  gutter: 24px
  card-gap: 24px
  section-margin: 48px
---

## Brand & Style

The design system is engineered for high-stakes business management, prioritizing clarity, trust, and rhythmic efficiency. The brand personality is "Grounded Professionalism"—it avoids the coldness of typical enterprise software by incorporating organic, earthy tones that evoke stability and reliability.

The aesthetic follows a **Modern Corporate** style with **Minimalist** and **Tactile** influences. It utilizes a card-based architecture to compartmentalize complex data into digestible modules. High contrast is maintained across all functional elements to ensure accessibility and rapid information processing, while soft depth markers provide a sense of physical organization.

## Colors

The palette is anchored by a deep Umber primary and a vibrant Ochre secondary. These earthy accents are balanced against a clinical white and light-gray foundation to maintain a "clean" professional feel.

- **Primary (#7D4A25):** Used for navigation states, primary actions, and key brand touchpoints.
- **Secondary (#E87C31):** Reserved for call-to-actions, notifications, and interactive highlights to draw the eye.
- **Backgrounds:** The main workspace uses a soft off-white (#F9F9F9) to reduce eye strain, while cards are pure white (#FFFFFF) to pop against the surface.
- **Borders:** A subtle, low-contrast gray (#E2E2E2) defines structure without creating visual noise.

## Typography

This design system utilizes **Be Vietnam Pro** to ensure native-level support for Vietnamese diacritics while maintaining a modern, international SaaS aesthetic. The font's geometric yet friendly construction fits the "Grounded Professionalism" theme perfectly.

- **Headlines:** Use Bold or SemiBold weights with tighter letter-spacing to create a strong visual hierarchy.
- **Body Text:** Standardized at 14px and 16px for optimal legibility in data-heavy environments.
- **Labels:** Use Medium weight and slight letter-spacing for all-caps or small-scale metadata.
- **Vietnamese Optimization:** Line heights are slightly increased (1.5x) for body text to accommodate stacked tone marks without overlap.

## Layout & Spacing

The layout employs a **Fluid Grid** system within a max-width container (1440px) to ensure the dashboard feels expansive yet controlled. 

- **Grid:** A 12-column system is used for desktop, collapsing to 1 column for mobile.
- **Rhythm:** An 8px linear scale governs all padding and margins. 
- **Card Layout:** Information is grouped into cards with 24px internal padding and 24px external gaps. 
- **Responsive Behavior:** On mobile, side margins reduce from 32px to 16px, and large display fonts scale down to `headline-lg-mobile` specs to maintain readability.

## Elevation & Depth

Depth is used sparingly to signify interactivity and priority. This design system moves away from flat design toward a "Soft-Tactile" approach.

- **Level 0 (Surface):** The background (#F9F9F9).
- **Level 1 (Default Card):** Pure white surface with a very soft, diffused shadow: `0px 4px 20px rgba(0, 0, 0, 0.04)`.
- **Level 2 (Hover/Active):** Increased shadow depth to signify lift: `0px 12px 32px rgba(125, 74, 37, 0.08)`. Note the subtle tint of the primary color in the shadow to maintain warmth.
- **Outlines:** All cards and containers feature a 1px solid border (#E2E2E2) to ensure edge definition regardless of screen calibration.

## Shapes

The design system utilizes a generous corner radius to soften the "industrial" nature of business data.

- **Primary Radius:** 24px (`rounded-xl`) is the standard for all main data cards and modal containers.
- **Component Radius:** 8px (`rounded-md`) is used for buttons, input fields, and smaller UI elements to maintain a professional, sharp look where precision is required.
- **Interactive States:** Buttons transition from standard corners to slightly more rounded shapes on active states to provide tactile feedback.

## Components

### Buttons
- **Primary:** Solid #7D4A25 with White text. 8px border radius. Medium weight text.
- **Secondary:** Outlined with #E2E2E2, text in #1A1A1A.
- **Action:** High-contrast #E87C31 for urgent actions (e.g., "Create New").

### Cards
- Standard containers for all dashboard modules. Must have 24px padding and 24px corner radius. Headlines inside cards should be `title-lg`.

### Input Fields
- Background: Pure White.
- Border: 1px #E2E2E2. 
- Focus State: 2px solid #E87C31 with a soft orange outer glow.
- Labels: `label-md` in 60% opacity neutral.

### Lists & Tables
- Clean, row-based layouts with #E2E2E2 horizontal dividers. 
- Zebra-striping is avoided; instead, use hover states with a #F4F1EE background fill.

### Chips & Badges
- Used for status (e.g., "Pending", "Complete"). 
- Use low-saturation versions of the primary colors (e.g., a very pale orange background with dark orange text) for high readability without visual fatigue.