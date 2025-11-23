# Tools Menu - "Match" Items Documentation

## Discovery Date
Generated during initial exploration test.

## Navigation Path
1. Login to portal
2. Enter "MOCK" in companies field on `/login_company` page
3. Navigate to `/run` (main application page)
4. Click "Tools" in top navigation bar
5. Tools dropdown menu opens

## Match Menu Items Found

### 1. Match Projects
- **Text**: "Match Projects"
- **URL**: `/tools/match_tools/get_project_match_data`
- **Selector**: `.dropdown-menu a[href='/tools/match_tools/get_project_match_data']`
- **Alternative**: `[role='menuitem']` with text "Match Projects"

### 2. Match Commitments
- **Text**: "Match Commitments"
- **URL**: `/tools/match_tools/get_commitment_match_data`
- **Selector**: `.dropdown-menu a[href='/tools/match_tools/get_commitment_match_data']`
- **Alternative**: `[role='menuitem']` with text "Match Commitments"

## Implementation Notes

### Tools Menu Selector
- **Tools Link**: `text=/^Tools$/i` or `a:has-text('Tools')`
- **Menu Container**: `.dropdown-menu`

### Best Selectors for Testing
- Use `.dropdown-menu a` to find all dropdown items
- Use `[role='menuitem']` for accessibility-friendly selection
- Filter by href attribute for specific items

## Test Strategy
- Verify menu items are visible after clicking Tools
- Test navigation to each destination page
- Verify destination pages load successfully
- Test from the /run page (requires company selection first)
