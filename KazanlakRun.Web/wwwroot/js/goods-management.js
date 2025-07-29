
 let rowData = [];
let nextId = 1;

const columnDefs = [
            { headerName: 'Select', checkboxSelection: true, headerCheckboxSelection: true, width: 90, pinned: 'left', suppressMenu: true, suppressSorting: true },
            { headerName: 'ID', field: 'id', width: 80, editable: false, cellClass: 'text-center', type: 'numericColumn' },
            { headerName: 'Name', field: 'name', editable: true, flex: 2, cellEditor: 'agTextCellEditor', cellEditorParams: { maxLength: 100 }, cellClassRules: { 'invalid-cell': params => !params.value || params.value.trim() === '' } },
            { headerName: 'Measure', field: 'measure', editable: true, flex: 1, cellEditor: 'agTextCellEditor', cellEditorParams: { maxLength: 20 }, cellClassRules: { 'invalid-cell': params => !params.value || params.value.trim() === '' } },
            { headerName: 'Available quantity in stock', field: 'quantity', editable: true, width: 120, type: 'numericColumn', cellEditor: 'agTextCellEditor', cellClassRules: { 'invalid-cell': params => isNaN (params.value) }, valueParser: params => Number (params.newValue) },
              {
headerName: 'Quantity for one runner per point',
              field: 'quantityOneRunner',
              editable: true,
              width: 140,
               type: 'numericColumn',
               cellEditor: 'agTextCellEditor',
               cellClassRules:
    {
        'invalid-cell': params => isNaN (params.value)
               },
               valueParser: params => Number (params.newValue)
               }
                ];

const gridOptions = {
            headerHeight: 60,
            columnDefs,
            rowData,
            defaultColDef: { sortable: true, filter: true, resizable: true },
            rowSelection: 'multiple', suppressRowClickSelection: true, enableCellChangeFlash: true, undoRedoCellEditing: true,
            enableRangeSelection: true, enableFillHandle: true, enableRangeHandle: true, enableClipboard: true,
            onCellValueChanged: onCellValueChanged, onRowDataChanged: onRowDataChanged,
            getContextMenuItems: getContextMenuItems,
            wrapHeaderText: true,
            autoHeaderHeight: true
        };

let gridApi;

document.addEventListener('DOMContentLoaded', () => {
    const gridDiv = document.querySelector('#myGrid');
    gridApi = agGrid.createGrid(gridDiv, gridOptions);
    document.addEventListener('keydown', handleKeyDown);

    fetch('/api/GoodsApi')
        .then(res => res.json())
        .then(data => {
            rowData = data;
            gridApi.setRowData(rowData);
            nextId = data.length ? Math.max(...data.map(g => g.id)) + 1 : 1;
        })
        .catch(err => showStatus('❌ Error loading data: ' + err.message, 'error'));
});

function onCellValueChanged(event) { showStatus('Cell updated. Don\'t forget to save!', 'success'); }
function onRowDataChanged() { }
function getContextMenuItems(params) { return ['copy', 'paste', 'separator', { name: 'Add Row', action: addRow }, { name: 'Delete Row', action: () => deleteRow (params.node.data.id) }]; }
function handleKeyDown(event)
{
    if (event.key === 'Delete' && !event.target.classList.contains('ag-cell-edit-input')) deleteSelected();
    if (event.ctrlKey && event.key === 's') { event.preventDefault(); saveChanges(); }
}

function deleteRow(id)
{
    const node = gridApi.getRowNode(id.toString());
    if (node) { gridApi.applyTransaction({ remove: [node.data] }); showStatus(`Deleted ID ${ id}. Save to confirm.`, 'success'); }
}
function deleteSelected()
{
    const selected = gridApi.getSelectedRows();
    if (!selected.length) { showStatus('Select rows to delete.', 'error'); return; }
    if (confirm(`Delete ${ selected.length}
    row(s) ?`)) { gridApi.applyTransaction({ remove: selected }); showStatus(`${ selected.length} deleted.Save to confirm.`, 'success'); }
}

function showStatus(msg, type)
{
    const s = document.getElementById('status');
    s.textContent = msg;
    s.className = `status ${ type}`;
    s.style.display = 'block';
    setTimeout(() => s.style.display = 'none', 5000);
}
async function saveChanges()
{
    const all = [];
    gridApi.forEachNode(n => n.data && all.push(n.data));
    const invalid = all.filter(item => {
        return !item.name || !item.name.trim() ||
            !item.measure || !item.measure.trim() ||
            item.quantity === null || item.quantity === undefined || isNaN(item.quantity) ||
            item.quantityOneRunner === null || item.quantityOneRunner === undefined || isNaN(item.quantityOneRunner);
    });

    if (invalid.length)
    {
        showStatus(`❌ ${ invalid.length}
        record(s) have invalid data.Please check all fields.`, 'error');
        console.log('Invalid records:', invalid);
        return;
    }
    const names = all.map(item => item.name.trim().toLowerCase());
    const duplicateNames = names.filter((name, index) => names.indexOf(name) !== index);
    if (duplicateNames.length > 0)
    {
        showStatus('❌ Duplicate names found: ' + [...new Set(duplicateNames)].join(', '), 'error');
        return;
    }

    try
    {
        showStatus('💾 Saving changes...', 'success');

        const res = await fetch('/api/GoodsApi/batch', {
        method: 'POST',
                    headers:
            {
                'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(all)
                });

if (!res.ok)
{
    const errorText = await res.text();
    throw new Error(`HTTP ${ res.status }: ${ errorText}`);
}

const data = await res.json();
gridApi.setRowData(data);
nextId = data.length ? Math.max(...data.map(g => g.id)) + 1 : 1;

showStatus('✅ All changes saved successfully!', 'success');

            } catch (e) {
    console.error('Save error:', e);
    showStatus('❌ Error saving: ' + e.message, 'error');
}
        }
        function addRow()
{
    const newRow = {
                id: 0,
                name: '',
                measure: '',
                quantity: 0,
                quantityOneRunner: 0
           
            };

gridApi.applyTransaction({ add: [newRow], addIndex: 0 });
showStatus('➕ New good added. Fill in the details and save.', 'success');
        }
    