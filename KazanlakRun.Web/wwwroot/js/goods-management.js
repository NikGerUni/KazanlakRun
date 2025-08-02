// --- CSRF helper ---
function getCsrfToken() {
    const el = document.querySelector('meta[name="csrf-token"]');
    return el ? el.getAttribute('content') : null;
}

/**
 * Wrapper около fetch, който добавя anti-forgery token и cookies.
 * @param {string} url 
 * @param {object} options 
 * @returns {Promise<Response>}
 */
async function fetchWithCsrf(url, options = {}) {
    const headers = new Headers(options.headers || {});

    const token = getCsrfToken();
    if (token) {
        headers.set('RequestVerificationToken', token);
    }

    const opts = {
        credentials: 'same-origin', // за да се изпращат cookies (auth)
        ...options,
        headers,
    };

    return fetch(url, opts);
}

// --- UI / grid state ---
let rowData = [];
let nextId = 1;
let gridApi;

// дефиниция на колоните
const columnDefs = [
    {
        headerName: 'Select',
        checkboxSelection: true,
        headerCheckboxSelection: true,
        width: 90,
        pinned: 'left',
        suppressMenu: true,
        suppressSorting: true
    },
    { headerName: 'ID', field: 'id', width: 80, editable: false, cellClass: 'text-center', type: 'numericColumn' },
    {
        headerName: 'Name',
        field: 'name',
        editable: true,
        flex: 2,
        cellEditor: 'agTextCellEditor',
        cellEditorParams: { maxLength: 100 },
        cellClassRules: { 'invalid-cell': params => !params.value || params.value.trim() === '' }
    },
    {
        headerName: 'Measure',
        field: 'measure',
        editable: true,
        flex: 1,
        cellEditor: 'agTextCellEditor',
        cellEditorParams: { maxLength: 20 },
        cellClassRules: { 'invalid-cell': params => !params.value || params.value.trim() === '' }
    },
    {
        headerName: 'Available quantity in stock',
        field: 'quantity',
        editable: true,
        width: 120,
        type: 'numericColumn',
        cellEditor: 'agTextCellEditor',
        cellClassRules: { 'invalid-cell': params => isNaN(params.value) },
        valueParser: params => Number(params.newValue)
    },
    {
        headerName: 'Quantity for one runner per point',
        field: 'quantityOneRunner',
        editable: true,
        width: 140,
        type: 'numericColumn',
        cellEditor: 'agTextCellEditor',
        cellClassRules: { 'invalid-cell': params => isNaN(params.value) },
        valueParser: params => Number(params.newValue)
    }
];

// grid options
const gridOptions = {
    headerHeight: 60,
    columnDefs,
    rowData,
    defaultColDef: { sortable: true, filter: true, resizable: true },
    rowSelection: 'multiple',
    suppressRowClickSelection: true,
    enableCellChangeFlash: true,
    undoRedoCellEditing: true,
    enableRangeSelection: true,
    enableFillHandle: true,
    enableRangeHandle: true,
    enableClipboard: true,
    onCellValueChanged: onCellValueChanged,
    onRowDataChanged: onRowDataChanged,
    getContextMenuItems: getContextMenuItems,
    wrapHeaderText: true,
    autoHeaderHeight: true,
    getRowNodeId: data => data.id?.toString() ?? Math.random().toString() // за да поддържа идентификатори
};

// --- DOM и инициализация ---
document.addEventListener('DOMContentLoaded', () => {
    const gridDiv = document.querySelector('#myGrid');
    gridApi = agGrid.createGrid(gridDiv, gridOptions); // според версията

    document.addEventListener('keydown', handleKeyDown);

    loadInitialData();
});

// зареждане от API
async function loadInitialData() {
    try {
        const res = await fetchWithCsrf('/api/GoodsApi'); // GET, токен не е задължителен за GET, но wrapper-а го добавя ако има
        if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
        const data = await res.json();
        rowData = data;
        gridApi.setRowData(rowData);
        nextId = data.length ? Math.max(...data.map(g => g.id)) + 1 : 1;
        showStatus('🟢 Data loaded', 'success');
    } catch (err) {
        showStatus('❌ Error loading data: ' + err.message, 'error');
        console.error('Load error:', err);
    }
}

// --- Event handlers ---
function onCellValueChanged(event) {
    showStatus("Cell updated. Don't forget to save!", 'success');
}

function onRowDataChanged() {
    // може да се използва за допълнителна логика, ако трябва
}

function getContextMenuItems(params) {
    return [
        'copy',
        'paste',
        'separator',
        { name: 'Add Row', action: addRow },
        { name: 'Delete Row', action: () => deleteRow(params.node.data.id) }
    ];
}

function handleKeyDown(event) {
    if (event.key === 'Delete' && !event.target.classList.contains('ag-cell-edit-input')) {
        deleteSelected();
    }
    if (event.ctrlKey && (event.key === 's' || event.key === 'S')) {
        event.preventDefault();
        saveChanges();
    }
}

// --- CRUD-like операции ---
function addRow() {
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

function deleteRow(id) {
    const node = gridApi.getRowNode(id?.toString());
    if (node) {
        gridApi.applyTransaction({ remove: [node.data] });
        showStatus(`Deleted ID ${id}. Save to confirm.`, 'success');
    }
}

function deleteSelected() {
    const selected = gridApi.getSelectedRows();
    if (!selected.length) {
        showStatus('Select rows to delete.', 'error');
        return;
    }
    if (confirm(`Delete ${selected.length} row(s)?`)) {
        gridApi.applyTransaction({ remove: selected });
        showStatus(`${selected.length} deleted. Save to confirm.`, 'success');
    }
}

// --- Validation и save ---
function validateAll(items) {
    const invalid = items.filter(item => {
        return !item.name || !item.name.trim() ||
            !item.measure || !item.measure.trim() ||
            item.quantity === null || item.quantity === undefined || isNaN(item.quantity) ||
            item.quantityOneRunner === null || item.quantityOneRunner === undefined || isNaN(item.quantityOneRunner);
    });
    return invalid;
}

function findDuplicateNames(items) {
    const names = items.map(i => i.name.trim().toLowerCase());
    return names.filter((name, idx) => names.indexOf(name) !== idx);
}

async function saveChanges() {
    const all = [];
    gridApi.forEachNode(n => {
        if (n.data) all.push(n.data);
    });

    // валидация
    const invalid = validateAll(all);
    if (invalid.length) {
        showStatus(`❌ ${invalid.length} record(s) have invalid data. Please check all fields.`, 'error');
        console.log('Invalid records:', invalid);
        return;
    }

    const duplicateNames = [...new Set(findDuplicateNames(all))];
    if (duplicateNames.length > 0) {
        showStatus('❌ Duplicate names found: ' + duplicateNames.join(', '), 'error');
        return;
    }

    try {
        showStatus('💾 Saving changes...', 'success');

        const res = await fetchWithCsrf('/api/GoodsApi/batch', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(all)
        });

        if (!res.ok) {
            const errorText = await res.text();
            throw new Error(`HTTP ${res.status}: ${errorText}`);
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

// --- UI feedback ---
function showStatus(msg, type) {
    const s = document.getElementById('status');
    if (!s) return;
    s.textContent = msg;
    s.className = `status ${type}`;
    s.style.display = 'block';
    clearTimeout(s._timeout);
    s._timeout = setTimeout(() => {
        s.style.display = 'none';
    }, 5000);
}
