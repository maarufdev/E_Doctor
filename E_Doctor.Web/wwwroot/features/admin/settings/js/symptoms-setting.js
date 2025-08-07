(function () {
    const SAMPLE = {
        symptoms: [
            { id: 1, name: 'Fever' }, { id: 2, name: 'Cough' }, { id: 3, name: 'Sore Throat' },
            { id: 4, name: 'Headache' }, { id: 5, name: 'Body Aches' }, { id: 6, name: 'Rash' },
            { id: 7, name: 'Itchiness' }, { id: 8, name: 'Fatigue' }, { id: 9, name: 'Difficulty Breathing' },
            { id: 10, name: 'Nausea' }, { id: 11, name: 'Vomiting' }
        ]
    }
    const URLS = {

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
            symptomsTblBody: "#symptoms-table-body",
            actionBtns: {
                edit: "",
                delete: ""
            },
        },
        modals: {
            symptomsModal: {
                root: "#symptom-modal",
                title: "#symptom-modal-title",
                fields: {
                    symptomId: "#symptom-id-input",
                    symptomName: "#symptom-name-input"
                },
                buttons: {
                    save: "#save-symptom-btn",
                    close: "#close-symptom-modal-btn"
                },
            },
        },
        buttons: {
            newSymptom: "#new-symptom-btn"
        },
    }

    const services = {
        initialize: function () {
            services.events.initMain();
            services.eventHandlers.renderSymptomsTable();
        },
        eventHandlers: {
            toggleSymptomsModal: function (toOpen, toEdit) {
                const { modals } = elementHolders;
                $(modals.symptomsModal.root).toggleClass("visible", toOpen);
                $(modals.symptomsModal.title).text(toEdit ? "Edit Symptom" : "New Symptom");
            },
            populateSymptomForm: function (symptom) {
                if (IsNullUndefinedEmpty(symptom)) return;

                const { symptomsModal } = elementHolders.modals;
                $(symptomsModal.fields.symptomId).val(symptom.symptomId ?? "");
                $(symptomsModal.fields.symptomName).val(symptom.symptomName ?? "");
            },
            handleOnClickEditSymptom: async function (symptomId) {
                const { apiService, eventHandlers } = services;
                const symptom = await apiService.getSymptomById(symptomId);
                eventHandlers.toggleSymptomsModal(true, true);
            },
            updateSymptomForm: function () {

            },
            renderSymptomsTable: async function () {
                const { eventHandlers } = services;

                const { table } = elementHolders;
                const symptomsTableBody = document.querySelector(table.symptomsTblBody);

                if (IsNullUndefinedEmpty(symptomsTableBody)) {

                    return;
                }

                const symptoms = await services.apiService.getSymptoms() ;

                symptomsTableBody.innerHTML = "";

                symptoms.forEach(symptom => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${symptom.name}</td>
                        <td class="actions-cell">
                            <button class="btn btn-warning edit-symptom-btn" style="padding: 0.5rem;" data-id="${symptom.id}" title="Edit"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg></button>
                            <button class="btn btn-danger delete-symptom-btn" style="padding: 0.5rem;" data-id="${symptom.id}" title="Delete"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg></button>
                        </td>
                    `;

                    row.querySelector('.edit-symptom-btn').addEventListener('click', (e) => {
                        const symptomId = e.currentTarget.dataset.id;
                        eventHandlers.handleOnClickEditSymptom(symptomId);
                    });

                    row.querySelector('.delete-symptom-btn').addEventListener('click', (e) => {
                        const isYes = confirm("Are you sure? Deleting a symptom will also affect disease rules");

                        if (isYes) {
                            alert("Symptom was deleted!");
                        }

                    });
                    symptomsTableBody.appendChild(row);
                });
            },
        },
        events: {
            initMain: function () {
                const { eventHandlers } = services;
                const { buttons, modals } = elementHolders;

                $(buttons.newSymptom).off("click").on("click", function () {
                    eventHandlers.toggleSymptomsModal(true);
                });
                $(modals.symptomsModal.buttons.close).off("click").on("click", function () {
                    eventHandlers.toggleSymptomsModal(false);
                });

                $(modals.symptomsModal.buttons.save).off("click").on("click", function () {
                    eventHandlers.toggleSymptomsModal(false);
                });
            }
        },
        apiService: {
            getSymptoms: async () => {
                return SAMPLE.symptoms;
            },
            getSymptomById: async (symptomId) => {
                return SAMPLE.symptoms.find(s => s.id == symptomId);
            },
            saveSymptom: async () => {
                
            },
            deleteSymptom: async (symptomId) => {
                console.log(`deleting symptom`)
            }
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();