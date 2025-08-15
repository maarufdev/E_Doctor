const DISEASES = [
    { id: 1, name: 'Common Cold', description: 'A common viral infection of the nose and throat.', rules: [{ symptomId: 1, condition: 'equal', value: 1 }, { symptomId: 2, condition: 'equal', value: 1 }] },
    { id: 2, name: 'Influenza (Flu)', description: 'A contagious respiratory illness caused by influenza viruses.', rules: [{ symptomId: 1, condition: 'equal', value: 1 }, { symptomId: 4, condition: 'equal', value: 1 }, { symptomId: 5, condition: 'more_than', value: 2 }] },
    { id: 3, name: 'Allergic Reaction', description: 'The body\'s reaction to a normally harmless substance.', rules: [{ symptomId: 6, condition: 'equal', value: 1 }, { symptomId: 7, condition: 'equal', value: 1 }] }
];
const SYMPTOMS = [
    { id: 1, name: 'Fever' },
    { id: 2, name: 'Cough' },
    { id: 3, name: 'Sore Throat' },
    { id: 4, name: 'Headache' },
    { id: 5, name: 'Body Aches' },
    { id: 6, name: 'Rash' },
    { id: 7, name: 'Itchiness' },
    { id: 8, name: 'Fatigue' },
    { id: 9, name: 'Difficulty Breathing' },
    { id: 10, name: 'Nausea' },
    { id: 11, name: 'Vomiting' }
];

(function () {
    const SYMPTOM_BASE_URL = "AdminSetting";

    const URLS = {
        getDiseases: `${SYMPTOM_BASE_URL}/GetDiseases`,
        getSymptomById: `${SYMPTOM_BASE_URL}/GetSymptomById`,
        saveSymptom: `${SYMPTOM_BASE_URL}/SaveSymptom`,
        deleteSymptom: `${SYMPTOM_BASE_URL}/RemoveSymptom`,
    }
    const stateHolders = {
        selectedRules: [],
        commandQueries: {
            saveSymptom: {
                symptomId: null,
                symptomName: ""
            },
            symptomId: null,
        }
    }
    const elementHolders = {
        table: {
            diseasesTableBody: "#diseases-table-body",
            actionBtns: {
                edit: ".edit-disease-btn",
                delete: ".delete-disease-btn"
            },
        },
        forms: {
            disease: {
                root: "#disease-modal",
                title: "disease-modal-title",
                rulesContainer: "#disease-rules-container",
                ddSymptom: "#symptom-select",
                btnAddDiseaseRule: "#add-disease-rule-btn",
                rulesContainer: "#disease-rules-container",
                saveBtn: "#save-disease-btn",
                closeBtn: "#close-disease-modal",
            },
        },
        buttons: {
            newDiseaseBtn: "#new-disease-btn"
        },
    }

    const services = {
        initialize: function () {
            services.events.initMain();
            services.eventHandlers.populateSymptomSelect();
            services.events.disease();
            services.eventHandlers.renderDiseaseTable();
        },
        eventHandlers: {
            toggleDiseasesModal: function (toOpen, toEdit) {
                const { forms } = elementHolders;
                $(forms.disease.root).toggleClass("visible", toOpen);
                $(forms.disease.title).text(toEdit ? "Edit Symptom" : "New Symptom");
            },
            renderDiseaseTable: async function () {
                const { table } = elementHolders;

                const diseaseModal = this.toggleDiseasesModal;

                const diseases = DISEASES;
                const $diseasesTableBody = $(table.diseasesTableBody);
                $diseasesTableBody.empty();

                diseases.forEach(disease => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${disease.name}</td>
                        <td style="white-space: normal;">${disease.description}</td>
                        <td>${disease.rules.length}</td>
                        <td class="actions-cell">
                            <button class="btn btn-warning edit-disease-btn" style="padding: 0.5rem;" data-id="${disease.id}" title="Edit"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg></button>
                            <button class="btn btn-danger delete-disease-btn" style="padding: 0.5rem;" data-id="${disease.id}" title="Delete"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg></button>
                        </td>
                    `;
                    row.querySelector(table.actionBtns.edit).addEventListener('click', (e) => {
                        if (disease) {
                            diseaseModal(true, true);
                        }
                    });
                    row.querySelector(table.actionBtns.delete).addEventListener('click', (e) => {
                        if (confirm('Are you sure?')) {
                           alert("Successfully deleted!")
                        }
                    });
                    $diseasesTableBody.append(row);
                });
            },
            renderDiseaseRules: function () {
            },
            createRuleFieldConfig: function (data) {

                const ruleRow = document.createElement('div');
                ruleRow.className = 'flex items-center gap-2 p-2';
                ruleRow.style.backgroundColor = '#f9fafb';
                ruleRow.style.borderRadius = '0.25rem';
                ruleRow.dataset.symptomId = data.symptomId;
                ruleRow.innerHTML = `
                        <input type="text" value="${data.name}" class="form-input" style="background-color: #e5e7eb; flex-grow: 1;" readonly>
                        <select class="form-select condition-select" style="width: 10rem;">
                            <option value="more_than" ${data.condition === 'more_than' ? 'selected' : ''}>More Than</option>
                            <option value="less_than" ${data.condition === 'less_than' ? 'selected' : ''}>Less Than</option>
                            <option value="equal" ${data.condition === 'equal' ? 'selected' : ''}>Equal to</option>
                        </select>
                        <input type="number" value="${data.value}" min="1" class="form-input value-input" style="width: 6rem;">
                        <button class="remove-rule-btn" style="background:none; border:none; color:var(--danger-color); cursor:pointer; font-size:1.5rem; line-height:1;">&times;</button>
                    `;

                return ruleRow;
            },
            addRule: function () {
                const { forms } = elementHolders;
                const $ddSymptom = $(forms.disease.ddSymptom);
                const symptomId = parseInt($ddSymptom.val());

                if (symptomId == 0) return;

                const symptomName = $ddSymptom.find("option:selected").text();

                const symptom = {
                    symptomId: symptomId,
                    name: symptomName,
                    condition: "equal",
                    value: 1,
                }

                const $ruleFieldsContainer = $(forms.disease.rulesContainer);

                $ruleFieldsContainer.append(this.createRuleFieldConfig(symptom));
                stateHolders.selectedRules.push(symptom);

                this.populateSymptomSelect();
            },
            populateSymptomSelect: function () {
                const { forms } = elementHolders;
                const $ddSymptom = $(forms.disease.ddSymptom);
                $ddSymptom.val("");
                $ddSymptom.empty();

                const $defaultOption = $(`<option value="0">-- Select -- </option>`);
                $ddSymptom.append($defaultOption);

                const filteredSymptoms = SYMPTOMS.filter(s => !stateHolders.selectedRules.some(r => r.symptomId == s.id));
                const symptomOptions = filteredSymptoms.map(s => `<option value="${s.id}">${s.name}</option>`).join('');
                $ddSymptom.append(symptomOptions);
            },
            handleOnRemoveSymptomRule: function ($row) {
                const symptomId = $row.data("symptom-id");
                const currentSelectedSymptomRules = stateHolders.selectedRules.filter(x => x.symptomId != symptomId);
                stateHolders.selectedRules = currentSelectedSymptomRules;

                $row.remove();

                this.populateSymptomSelect();
            },
        },
        events: {
            initMain: function () {
                const { buttons } = elementHolders;
                const { eventHandlers } = services;

                registerEvent(
                    buttons.newDiseaseBtn,
                    "click",
                    function (e) {
                        eventHandlers.toggleDiseasesModal(true, false);
                    }
                )
            },
            disease: function () {
                const { forms } = elementHolders;
                const { eventHandlers } = services;

                registerEvent(
                    forms.disease.closeBtn,
                    "click",
                    function (e) {
                        eventHandlers.toggleDiseasesModal(false);
                    }
                );

                registerEvent(
                    forms.disease.btnAddDiseaseRule,
                    "click",
                    function (e) {
                        eventHandlers.addRule();
                    }
                );

                registerEvent(
                    forms.disease.rulesContainer,
                    "click",
                    function (e) {
                        const $target = $(e.target);

                        if ($target.hasClass("remove-rule-btn")) {
                            const $row = $target.closest("[data-symptom-id]");
                            eventHandlers.handleOnRemoveSymptomRule($row);
                        }
                    }
                );
            }
        },
        apiService: {
           
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();