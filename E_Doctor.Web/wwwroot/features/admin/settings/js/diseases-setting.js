const DISEASES = [
    { id: 1, name: 'Common Cold', description: 'A common viral infection of the nose and throat.', rules: [{ symptomId: 1, condition: 'equal', value: 1 }, { symptomId: 2, condition: 'equal', value: 1 }] },
    { id: 2, name: 'Influenza (Flu)', description: 'A contagious respiratory illness caused by influenza viruses.', rules: [{ symptomId: 1, condition: 'equal', value: 1 }, { symptomId: 4, condition: 'equal', value: 1 }, { symptomId: 5, condition: 'more_than', value: 2 }] },
    { id: 3, name: 'Allergic Reaction', description: 'The body\'s reaction to a normally harmless substance.', rules: [{ symptomId: 6, condition: 'equal', value: 1 }, { symptomId: 7, condition: 'equal', value: 1 }] }
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
        modals: {
            disease: {
                root: "#disease-modal",
                title: "disease-modal-title",
                ruleContainer: "#disease-rules-containe",
                ddSymptom: "#symptom-select",
                btnAddDiseaseRule: "#add-disease-rule-btn",
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
            services.events.disease();
            services.eventHandlers.renderDiseaseTable();
        },
        eventHandlers: {
            toggleDiseasesModal: function (toOpen, toEdit) {
                const { modals } = elementHolders;
                $(modals.disease.root).toggleClass("visible", toOpen);
                $(modals.disease.title).text(toEdit ? "Edit Symptom" : "New Symptom");
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
            }
            
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
                const { modals } = elementHolders;
                const { eventHandlers } = services;

                registerEvent(
                    modals.disease.closeBtn,
                    "click",
                    function (e) {
                        eventHandlers.toggleDiseasesModal(false);
                    }
                )
            }
        },
        apiService: {
           
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();