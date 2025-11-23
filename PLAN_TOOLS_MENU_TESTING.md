# Plan: Tools Menu "Match" Items Testing

## Overview
Test all menu items under Tools → Match* by dynamically discovering and testing each one.

## Discovered Menu Items
From exploration test, we found:
- Match Projects → `/tools/match_tools/get_project_match_data`
- Match Commitments → `/tools/match_tools/get_commitment_match_data`

## Testing Approach (DRAFT - TO BE REFINED)

### Phase 1: Setup
1. Login to portal
2. Select "MOCK" company
3. Navigate to `/run` page
4. Open Tools menu

### Phase 2: Dynamic Discovery & Testing
**Instead of hardcoding menu names:**
1. Query all menu items in Tools dropdown that start with "Match"
2. Store their text and href attributes
3. Loop through each discovered item:
   - Click the menu item
   - Wait for page to load (configurable timeout - may be long)
   - Verify page loaded successfully (URL changed, no errors)
   - Navigate back to `/run`
   - Re-open Tools menu for next item

### Phase 3: Validation
For each "Match" menu item, verify:
- [ ] Menu item is visible in dropdown
- [ ] Menu item is clickable
- [ ] Navigation occurs (URL changes)
- [ ] Destination page loads (no errors, expected URL)
- [ ] Can return to /run page

## Decisions Made

### 1. Loading Time ✅
- **Timeout**: 60 seconds maximum
- **Behavior**: Fail test if load exceeds timeout

### 2. Page Load Verification ✅
- **Success criteria**:
  1. Page stops loading (network idle)
  2. Detect network request with "/v1/provider" in the URL path
  3. See table with `id="reactTable"` (final success indicator)

**Detailed Page Load Flow**:
1. **Click menu item** → Navigation starts
2. **Loading screen appears** → Text: "loading large data sets"
3. **Optional: "Select Project" Modal** (only on some screens)
   - If modal appears with title "Select Project":
     - Wait for modal table to load
     - Select first item in the modal table
     - Submit the modal
     - Continue to step 4
   - If no modal, continue to step 4
4. **Final table loads** → `id="reactTable"` becomes visible
5. **Success** = `reactTable` is visible
6. **Failure** = `reactTable` does not appear (indicates problem)

**Implementation Notes**:
- Must handle optional "Select Project" modal conditionally
- Primary success indicator: presence of `#reactTable`
- If `#reactTable` never appears, test fails

### 3. Navigation Flow ✅
- **Method**: Click Tools menu item with `href="/run"`
- **Not**: Browser back button or direct URL navigation
- **Benefit**: Mimics real user behavior

### 4. Test Structure ✅
**CHOSEN: Option A - Single Loop Test**
- One test method that discovers and loops through all "Match" items
- **Benefits**:
  - Simpler to maintain
  - More efficient (single login/setup)
  - Better for dynamic discovery
  - Easier to add logging/reporting for all items
  - Works perfectly with runtime discovery
  - Fail fast approach works well with loop

### 5. Failure Handling ✅
- **Approach**: Fail fast - stop on first failure
- **Reason**: Quicker feedback, less wasted time

### 6. Data-Driven vs Hardcoded ✅
- **Approach**: Runtime discovery
- **Reason**: Menus change over time and vary by company

## Final Implementation Plan

### Test Flow (Single Loop Test)
```
1. Setup (Once)
   ├─ Login
   ├─ Select "MOCK" company
   └─ Navigate to /run

2. Discover Match Items (Once)
   ├─ Open Tools menu
   ├─ Query all items starting with "Match"
   └─ Store: [{text, href}]

3. Loop Through Each Match Item
   ├─ Log: "Testing {item.text}"
   ├─ Click item
   ├─ Wait for "loading large data sets" (optional check)
   ├─ Check for "Select Project" modal
   │  ├─ If present:
   │  │  ├─ Wait for modal table to load
   │  │  ├─ Select first item
   │  │  ├─ Submit modal
   │  │  └─ Continue
   │  └─ If not present: Continue
   ├─ Wait for #reactTable (up to 60s)
   │  ├─ Success: reactTable visible → Log success
   │  └─ Failure: timeout or no table → Fail test (fail fast)
   ├─ Navigate back: Click Tools → item[href="/run"]
   ├─ Wait for /run page to load
   └─ Repeat for next item

4. Complete
   └─ Log: "All {count} Match items tested successfully"
```

### Key Selectors
- Tools menu: `text=/^Tools$/i`
- Tools dropdown: `.dropdown-menu`
- Match items: `.dropdown-menu a` (filter by text starting with "Match")
- Loading text: `text=/loading large data sets/i`
- Select Project modal: Modal with title/text "Select Project"
- Success table: `#reactTable`
- Return to run: `.dropdown-menu a[href="/run"]`

### Error Handling
- Screenshot on any failure
- Log which menu item failed
- Fail fast - stop on first failure
- Detailed console output for debugging

## Implementation Checklist (After Plan Approval)
- [ ] Create CompanySelectionPage.cs (if needed as separate page object)
- [ ] Update/Create ToolsMenuPage.cs with Match item discovery
- [ ] Create MatchScreenPage.cs for handling modal + reactTable
- [ ] Create ToolsMenuMatchTests.cs (single loop test)
- [ ] Implement network request listener for "/v1/provider"
- [ ] Implement modal detection and handling
- [ ] Implement 60-second timeout with reactTable verification
- [ ] Add detailed logging for each step
- [ ] Add screenshot capture on failure
- [ ] Test with MOCK company
- [ ] Document in CLAUDE.md

## Notes
- Do NOT hardcode menu item names in tests
- Account for long loading times with configurable timeouts
- Focus on dynamic discovery and flexible testing approach
