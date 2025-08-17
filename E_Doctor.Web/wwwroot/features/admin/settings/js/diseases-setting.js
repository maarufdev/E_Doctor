(function () {
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getRuleConditions: `${SETTINGS_BASE_URL}/GetRuleConditions`,
        getSymptoms: `${SETTINGS_BASE_URL}/GetSymptoms`,
        getDiseases: `${SETTINGS_BASE_URL}/GetDiseaseList`,
        getDiseaseById: `${SETTINGS_BASE_URL}/GetDiseaseById`,
        saveDisease: `${SETTINGS_BASE_URL}/SaveDisease`,
        removeDisease: `${SETTINGS_BASE_URL}/DeleteRuleById`,
    }
    const stateHolders = {
        tempSelectedRules: [],
        commandQueries: {
            saveDisease: {
                diseaseId: 0,
                diseaseName: "",
                description: "",
                rules: []
            },
            diseaseId: null,
        },
        symptoms: [],
        ruleConditions: [],
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
                fields: {
                    diseaseId: "#editing-disease-id",
                    diseaseName: "#disease-name-input",
                    description: "#disease-desc-input"
                },
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
            services.eventHandlers.setSymptomsList();
            services.eventHandlers.setRuleConditions();

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

                const $diseasesTableBody = $(table.diseasesTableBody);
                $diseasesTableBody.empty();

                const result = await services.apiService.getDiseases();
                result.forEach(disease => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${disease.diseaseName}</td>
                        <td style="white-space: normal;">${disease.description}</td>
                        <td>${disease.ruleCount}</td>
                        <td class="actions-cell">
                            <button class="btn btn-warning edit-disease-btn" style="padding: 0.5rem;" data-disease-id="${disease.diseaseId}" title="Edit"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg></button>
                            <button class="btn btn-danger delete-disease-btn" style="padding: 0.5rem;" data-disease-id="${disease.diseaseId}" title="Delete"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg></button>
                        </td>
                    `;
                    row.querySelector(table.actionBtns.edit).addEventListener('click', async (e) => {
                        if (disease) {
                            const result = await services.apiService.getDiseaseById(disease.diseaseId);

                            if (!result) {
                                alert("something went wrong!");
                                return;
                            }

                            services.eventHandlers.populateDiseaseOnForm(result);
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
            populateDiseaseOnForm: function (disease) {
                const { commandQueries } = stateHolders;
                const { fields } = elementHolders.forms.disease;

                const {
                    diseaseId,
                    diseaseName,
                    description,
                    rules
                } = disease ?? {};

                commandQueries.saveDisease.diseaseId = diseaseId;
                commandQueries.saveDisease.diseaseName = diseaseName;
                commandQueries.saveDisease.description = description;
                commandQueries.saveDisease.rules = rules ?? [];

                $(fields.diseaseId).val(diseaseId);
                $(fields.diseaseName).val(diseaseName);
                $(fields.description).val(description);

                this.constructSelectedRules();
            },
            constructSelectedRules: function () {
                const { forms } = elementHolders;
                const { commandQueries, tempSelectedRules, symptoms } = stateHolders;
                const diseaseRules = commandQueries.saveDisease.rules;

                const rulesToPopulates = diseaseRules.map(rule => {
                    const transformedRule = {
                        symptomId: rule.symptomId,
                        symptomName: symptoms.find(s => s.symptomId == rule.symptomId)?.symptomName ?? "",
                        condition: rule.condition,
                        days: rule.days,
                    }

                    tempSelectedRules.push(transformedRule);

                    return transformedRule;
                });

                const $ruleFieldsContainer = $(forms.disease.rulesContainer);

                $ruleFieldsContainer.empty();
                $ruleFieldsContainer.append(rulesToPopulates.map(symptom => this.createRuleFieldConfig(symptom)));

                this.populateSymptomSelect();
            },
            renderDiseaseRules: function () {
            },
            createRulesOption: function (selectedCondition) {
                const $select = $(`<select class="form-select rule-symptom-condition-select" style="width: 10rem;">`);

                const $options = stateHolders.ruleConditions.map(
                    c => `<option value="${c.id}" ${c.id == selectedCondition ? "selected" : ""}>${c.text}</option>`).join("");

                $select.append($options);

                return $select[0].outerHTML;
            },
            createRuleFieldConfig: function (data) {
                const ruleRow = document.createElement('div');
                ruleRow.className = 'flex items-center gap-2 p-2 rule-symptom-config-container';
                ruleRow.style.backgroundColor = '#f9fafb';
                ruleRow.style.borderRadius = '0.25rem';
                ruleRow.dataset.symptomId = data.symptomId;
                ruleRow.innerHTML = `
                        <input type="text" value="${data.symptomName}" class="form-input rule-symptom-text" style="background-color: #e5e7eb; flex-grow: 1;" readonly>
                        ${this.createRulesOption()}
                        <input type="number" value="${data.days}" min="1" class="form-input rule-symptom-days" style="width: 6rem;">
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
                    symptomName: symptomName,
                    condition: 1,
                    days: 1
                }

                const $ruleFieldsContainer = $(forms.disease.rulesContainer);

                $ruleFieldsContainer.append(this.createRuleFieldConfig(symptom));
                stateHolders.tempSelectedRules.push(symptom);

                this.populateSymptomSelect();
            },
            populateSymptomSelect: function () {
                const { forms } = elementHolders;
                const $ddSymptom = $(forms.disease.ddSymptom);
                $ddSymptom.val("");
                $ddSymptom.empty();

                const $defaultOption = $(`<option value="0">-- Select -- </option>`);
                $ddSymptom.append($defaultOption);

                const filteredSymptoms = stateHolders.symptoms.filter(s => !stateHolders.tempSelectedRules.some(r => r.symptomId == s.symptomId));
                const symptomOptions = filteredSymptoms.map(s => `<option value="${s.symptomId}">${s.symptomName}</option>`).join('');

                $ddSymptom.append(symptomOptions);
            },
            handleOnRemoveSymptomRule: function ($row) {
                const symptomId = $row.data("symptom-id");

                const currentSelectedSymptomRules = stateHolders.tempSelectedRules.filter(x => x.symptomId != symptomId);
                stateHolders.tempSelectedRules = currentSelectedSymptomRules;

                $row.remove();

                this.populateSymptomSelect();
            },
            resetDiseaseStates: function () {
                const { forms } = elementHolders;
                const { fields } = forms.disease;

                $(fields.diseaseId).val("");
                $(fields.diseaseName).val("");
                $(fields.description).val("");

                const $ruleFieldsContainer = $(forms.disease.rulesContainer);

                $ruleFieldsContainer.empty();

                stateHolders.commandQueries.saveDisease = {
                    diseaseId: 0,
                    diseaseName: "",
                    description: "",
                    rules: []
                };
                stateHolders.tempSelectedRules = [];
            },
            getDiseaseRules: function () {
                const $diseaseSymptomRules = $("#disease-rules-container").find(".rule-symptom-config-container");
                const rules = [...$diseaseSymptomRules].map(field => {
                    const symptomId = parseInt($(field).data("symptomId"));
                    const condition = parseInt($(field).find(".rule-symptom-condition-select").val());
                    const days = parseInt($(field).find(".rule-symptom-days").val());

                    return {
                        symptomId,
                        condition,
                        days
                    }
                });

                return rules;
            },
            handleOnSaveDisease: async function () {
                const { commandQueries, tempSelectedRules } = stateHolders;
                const { fields } = elementHolders.forms.disease;

                commandQueries.saveDisease.rules = this.getDiseaseRules();
                const valid = this.validateDiseaseFields();

                if (!valid) return;

                const result = await services.apiService.saveDisease(commandQueries.saveDisease);
                if (!result) {
                    alert("something went wrong!")
                    return;
                }

                await this.renderDiseaseTable();

                this.toggleDiseasesModal(false);
                this.resetDiseaseStates();
                $("#disease-rules-container").empty();
            },
            validateDiseaseFields: function () {
                const { commandQueries } = stateHolders;
                let result = true;

                result = ValidateInput(commandQueries.saveDisease.diseaseName, "Disease Name is required.");
                if (!result) return false;

                result = ValidateInput(commandQueries.saveDisease.rules, "Rules Fields is required. Please add config at least 1");
                if (!result) return false;

                return result;
            },
            setSymptomsList: async function () {
                const response = await services.apiService.getSymptoms();
                stateHolders.symptoms = response ?? []
            },
            setRuleConditions: async function () {
                const response = await services.apiService.getRuleConditions() ?? [];
                stateHolders.ruleConditions = response.map(x => ({ id: x.id, text: x.conditionName }));
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
                        eventHandlers.resetDiseaseStates();
                        eventHandlers.populateSymptomSelect();
                    }
                )
            },
            disease: function () {
                const { forms } = elementHolders;
                const { eventHandlers } = services;
                const { commandQueries } = stateHolders;

                registerEvent(
                    forms.disease.closeBtn,
                    "click",
                    function (e) {
                        eventHandlers.toggleDiseasesModal(false);
                        eventHandlers.resetDiseaseStates();
                        eventHandlers.populateSymptomSelect();
                    }
                );

                registerEvent(
                    forms.disease.saveBtn,
                    "click",
                    function (e) {
                        eventHandlers.handleOnSaveDisease();
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

                registerEvent(
                    forms.disease.fields.diseaseName,
                    "input",
                    function (e) {
                        const $target = $(e.target);
                        commandQueries.saveDisease.diseaseName = $target.val(); 
                    }
                );

                registerEvent(
                    forms.disease.fields.description,
                    "input",
                    function (e) {
                        const $target = $(e.target);
                        commandQueries.saveDisease.description = $target.val(); 
                    }
                );
            }
        },
        apiService: {
            getRuleConditions: async function () {
                return await apiFetch(URLS.getRuleConditions);
            },
            getSymptoms: async function () {
                return await apiFetch(URLS.getSymptoms);
            },
            getDiseases: async function () {
                return await apiFetch(URLS.getDiseases);
            },
            getDiseaseById: async function (diseaseId) {
                return await apiFetch(
                    URLS.getDiseaseById,
                    {
                        params: {
                            id: diseaseId
                        }
                    }    
                );
            },
            saveDisease: async function (command) {
                return await apiFetch(
                    URLS.saveDisease,
                    {
                        method: "POST",
                        body: command
                    }
                );
            },
            removeDisease: async function (diseaseId) {
                return await apiFetch(
                    URLS.removeDisease,
                    {
                        method: "DELETE",
                        params: {
                            id: diseaseId,
                        }
                    });
            },
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();