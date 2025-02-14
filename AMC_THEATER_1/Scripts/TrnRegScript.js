function makeDropdownReadonly(className) {
    let elements = document.querySelectorAll("." + className);
    elements.forEach(el => {
        el.addEventListener("mousedown", function (event) {
            event.preventDefault(); // Prevents opening the dropdown
        });

        el.style.backgroundColor = "#e9ecef"; // Light gray background (like readonly)
        el.style.pointerEvents = "none"; // Prevents clicking
        el.style.opacity = "1"; // Ensures visibility
        el.style.color = "#495057"; // Matches text color of readonly input
    });
}

document.addEventListener("DOMContentLoaded", function () {
    if (ViewBag.IsViewPage == true) {
         makeDropdownReadonly("readonly-field");
    }
});


// Function to toggle visibility of #offlineTaxDetails
function toggleFields(isOffline) {
    const offlineTaxDetails = document.getElementById('offlineTaxDetails');

    // Show the section if "Yes" is selected; hide it otherwise
    if (isOffline) {
        offlineTaxDetails.style.display = 'block';
    } else {
        offlineTaxDetails.style.display = 'none';
    }
}

// Attach event listener on DOMContentLoaded for initialization
document.addEventListener('DOMContentLoaded', () => {
    const yesRadio = document.getElementById('yes');
    const noRadio = document.getElementById('no');

    // Initialize visibility based on pre-selected option
    if (yesRadio.checked) {
        toggleFields(true);
    } else if (noRadio.checked) {
        toggleFields(false);
    }

    // Add click event listeners for runtime changes
    yesRadio.addEventListener('click', () => toggleFields(true));
    noRadio.addEventListener('click', () => toggleFields(false));
});


//SCREEN
function addScreen() {
    let screenCount = 1; // Initial screen count

    var table = document.getElementById("screenCapacityTable").getElementsByTagName('tbody')[0];
    var rowCount = table.rows.length;
    var newRow = table.insertRow(rowCount);

    newRow.innerHTML = ` <td>${rowCount + 1}</td>
                                                                                                                                                                                                                            <td><input type="number" name="seatCapacity" class="form-control ms-4" required oninput="updateScreenType(this)"></td>                                                                                                                                                                                                             <td><input type="text" name="screenType" class="form-control" me-1 readonly></td>
                                                                                                                                                                                                                            <td>
                                                                                                                                                                                                                                <button type="button" class="btn btn-primary me-2" onclick="addScreen()" title="Add Screen">
                                                                                                                                                                                                                                    <i class="fas fa-plus"></i>
                                                                                                                                                                                                                                </button>
                                                                                                                                                                                                                                <button type="button" class="btn btn-warning me-2" onclick="editRow(this)" title="Edit Screen">
                                                                                                                                                                                                                                    <i class="fas fa-edit"></i>
                                                                                                                                                                                                                                </button>
                                                                                                                                                                                                                                <button type="button" class="btn btn-danger me-2" onclick="removeRow(this)" title="Delete Screen">
                                                                                                                                                                                                                                    <i class="fas fa-trash-alt"></i>
                                                                                                                                                                                                                                                                                                                                                               `;
}

function removeRow(button) {
    var row = button.parentNode.parentNode;
    row.parentNode.removeChild(row);
}

function editRow(button) {
    var row = button.parentNode.parentNode;
    var seatCapacityInput = row.cells[1].querySelector("input");
    seatCapacityInput.removeAttribute("readonly");
}

function updateScreenType(input) {
    var seatCapacity = input.value;
    var row = input.parentNode.parentNode;
    var screenTypeInput = row.cells[2].querySelector("input");

    if (seatCapacity > 125) {
        screenTypeInput.value = "Theater";
    } else {
        screenTypeInput.value = "Video Theater";
    }
}

//FILE SIZE
function updateFileSize(docId) {
    const inputElement = document.getElementById(`file-${docId}`);
    const sizeElement = document.getElementById(`size-${docId}`);

    if (inputElement.files.length > 0) {
        // Get file size in bytes
        const fileSizeBytes = inputElement.files[0].size;

        // Convert to MB
        const fileSizeMB = (fileSizeBytes / (1024 * 1024)).toFixed(2);

        // Update size column
        sizeElement.textContent = `${fileSizeMB} MB`;

        // Validate file size (Max: 5 MB)
        if (fileSizeMB > 5) {
            alert("The selected file exceeds the maximum allowed size of 5 MB. Please select a smaller file.");
            inputElement.value = ""; // Clear input
            sizeElement.textContent = "0 MB"; // Reset size
        }
    } else {
        sizeElement.textContent = "0 MB";
    }
}
//VIEW BTN
function showViewButton(docId, input) {
    if (input.files.length > 0) {
        let file = input.files[0];
        let fileURL = URL.createObjectURL(file);

        document.getElementById(`viewBtn-${docId}`).innerHTML =
            `<a href='${fileURL}' target='_blank' class='btn btn-primary btn-sm'>View</a>`;
    }
}

//SCRENN

function addScreen() {
    let screenCount = 1; // Initial screen count

    const tableBody = document.getElementById('screenCapacityTable').querySelector('tbody');

    if (screenCount >= 10) {
        alert("You can add up to 10 screens only.");
        return;
    }

    const lastRow = tableBody.querySelector('tr:last-child');
    if (lastRow) {
        const seatCapacityInput = lastRow.querySelector('input[name="seatCapacity"]');
        if (seatCapacityInput && seatCapacityInput.value === '') {
            alert("Please fill in the seat capacity before adding a new screen.");
            return;
        }
        seatCapacityInput.readOnly = true;
    }

    screenCount++;
    const row = `    <tr>
                                                                                                                                                        <td>${screenCount}</td>
                                                                                                                                                        <td>
                                                                                                                                                            <input type="number" name="seatCapacity" class="form-control ms-4" required oninput="updateScreenType(this)">
                                                                                                                                                        </td>
                                                                                                                                                        <td>
                                                                                                                                                            <input type="text" name="screenType" class="form-control ms-4" readonly>
                                                                                                                                                        </td>
                                                                                                                                                        <td>
                                                                                                                                                            <button type="button" class="btn btn-primary me-2 " onclick="addScreen()">
                                                                                                                                                                <i class="fas fa-plus"></i>
                                                                                                                                                            </button>
                                                                                                                                                            <button type="button" class="btn btn-warning me-2" onclick="editRow(this)">
                                                                                                                                                                <i class="fas fa-edit"></i>
                                                                                                                                                            </button>
                                                                                                                                                            <button type="button" class="btn btn-danger me-2" onclick="removeRow(this)">
                                                                                                                                                                <i class="fas fa-trash-alt"></i>
                                                                                                                                                            </button>
                                                                                                                                                        </td>
                                                                                                                                                    </tr>
`;

    tableBody.insertAdjacentHTML('beforeend', row);
    updateAddScreenButtons();
}

function updateScreenType(input) {
    const seatCapacity = parseInt(input.value, 10);
    const row = input.closest('tr');
    const screenTypeInput = row.querySelector('input[name="screenType"]');

    if (!isNaN(seatCapacity)) {
        screenTypeInput.value = seatCapacity > 125 ? 'Theater' : 'Video Theater';
    } else {
        screenTypeInput.value = '';
    }
}

function editRow(button) {
    const row = button.closest('tr');
    const seatCapacityInput = row.querySelector('input[name="seatCapacity"]');

    if (seatCapacityInput.readOnly) {
        seatCapacityInput.readOnly = false;
        button.innerHTML = '<i class="fas fa-save"></i>';
        button.classList.remove("btn-warning");
        button.classList.add("btn-success");
    } else {
        seatCapacityInput.readOnly = true;
        button.innerHTML = '<i class="fas fa-edit"></i>';
        button.classList.remove("btn-success");
        button.classList.add("btn-warning");
    }
}

function removeRow(button) {
    const row = button.closest('tr');
    row.remove();
    updateScreenNumbers();

    const tableBody = document.getElementById('screenCapacityTable').querySelector('tbody');
    if (tableBody.querySelectorAll('tr').length === 0) {
        screenCount = 0;
        addScreen();
    }

    updateAddScreenButtons();
}

function updateScreenNumbers() {
    const rows = document.querySelectorAll('#screenCapacityTable tbody tr');
    screenCount = rows.length;

    rows.forEach((row, index) => {
        const screenNoCell = row.querySelector('td:first-child');
        screenNoCell.textContent = index + 1;
    });
}

function updateAddScreenButtons() {
    const rows = document.querySelectorAll('#screenCapacityTable tbody tr');
    rows.forEach((row, index) => {
        const addScreenButton = row.querySelector('.btn-primary');
        addScreenButton.style.display = index === rows.length - 1 ? "inline-block" : "none";
    });
}

function serializeScreenData() {
    const tableBody = document.getElementById('screenCapacityTable').querySelector('tbody');
    const rows = tableBody.querySelectorAll('tr');
    const screens = [];

    rows.forEach((row) => {
        const seatCapacity = row.querySelector('input[name="seatCapacity"]').value;
        const screenType = row.querySelector('input[name="screenType"]').value;
        screens.push({
            SeatCapacity: parseInt(seatCapacity),
            ScreenType: screenType
        });
    });

    document.getElementById('screenData').value = JSON.stringify(screens);
}

document.querySelector('form').addEventListener('submit', serializeScreenData);
updateAddScreenButtons();

