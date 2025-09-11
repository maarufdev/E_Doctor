(function () {
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getRuleConditions: `${SETTINGS_BASE_URL}/GetRuleConditions`,
        getIllnessSymptoms: `${SETTINGS_BASE_URL}/GetIllnessSymptoms`,
        getIllnessList: `${SETTINGS_BASE_URL}/GetIllnessList`,
        getIllnessById: `${SETTINGS_BASE_URL}/GetIllnessById`,
        saveIllness: `${SETTINGS_BASE_URL}/SaveIllness`,
        deleteIllnessById: `${SETTINGS_BASE_URL}/DeleteIllnessById`,
        getWeightRules: `${SETTINGS_BASE_URL}/GetWeightRules`,
        getExportRulesConfig: `${SETTINGS_BASE_URL}/GetExportRulesConfiguration`
    }
    const stateHolders = {
        tempSelectedRules: [],
        commandQueries: {
            saveIllness: {
                illnessId: 0,
                illnessName: "",
                description: "",
                rules: []
            },
            illnessId: null,
        },
        symptoms: [],
        ruleConditions: [],
        ruleWeights: [],
        searchParams: {
            searchText: "",
            pageNumber: 1,
            pageSize: 10
        },
        illnessPaginatedResult: {
            totalPages: 0,
        }
    }
    const elementHolders = {
        searchSection: {
            searchInput: "#illness-search-input",
            searchBtn: "#illness-search-btn",
            paginationSelect: "#illness-pagination"
        },
        table: {
            diseasesTableBody: "#diseases-table-body",
            actionBtns: {
                edit: ".edit-disease-btn",
                delete: ".delete-disease-btn"
            },
        },
        forms: {
            illness: {
                root: "#disease-modal",
                title: "#disease-modal-title",
                fields: {
                    illnessId: "#editing-disease-id",
                    illnessName: "#disease-name-input",
                    description: "#disease-desc-input"
                },
                rulesContainer: "#disease-rules-container",
                ddSymptom: "#symptom-select",
                btnAddIllnessRule: "#add-disease-rule-btn",
                rulesContainer: "#disease-rules-container",
                saveBtn: "#save-disease-btn",
                closeBtn: "#close-disease-modal",
            },
        },
        buttons: {
            newDiseaseBtn: "#new-disease-btn",
            exportRulesBtn: "#export-rules-btn",
        },
    }
    const services = {
        initialize: function () {
            services.eventHandlers.setSymptomsList();
            services.eventHandlers.setRuleConditions();
            services.eventHandlers.setWeightRules();

            services.events.searchEvent();
            services.events.initMain();
            services.eventHandlers.populateSymptomSelect();
            services.events.illnessEvent();
            services.eventHandlers.renderIllnessTable();
        },
        eventHandlers: {
            toggleIllnessModal: function (toOpen, toEdit) {
                const { forms } = elementHolders;
                $(forms.illness.root).toggleClass("visible", toOpen);
                $(forms.illness.title).text(toEdit ? "Edit Illness Details" : "Create New Illness");
            },
            handleOnExportRules: async function () {
                const result = await services.apiService.getExportRulesConfig();

                const jsonString = JSON.stringify(result, null, 2);
                const blob = new Blob([jsonString], { type: 'application/json' });

                triggerDownload(blob, 'rules-config.json');

                function triggerDownload(blob, filename) {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    a.download = filename;
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                }

            },
            createPaginations: function () {
                const { searchSection } = elementHolders;
                const { totalPages } = stateHolders.illnessPaginatedResult;

                const $pagination = $(searchSection.paginationSelect);
                $pagination.empty();

                for (let i = 1; i <= totalPages; i++) {
                    const $option = $(`<option>`);
                    $option.val = i;
                    $option.text(i);
                    $pagination.append($option);
                }

                $pagination.val(stateHolders.searchParams.pageNumber);
            },
            renderIllnessTable: async function () {
                const { table } = elementHolders;
                const diseaseModal = this.toggleIllnessModal;
                const eventHandlers = this;

                const $diseasesTableBody = $(table.diseasesTableBody);
                $diseasesTableBody.empty();

                const result = await services.apiService.getIllnessList();
                const illnessList = result.items ?? [];

                illnessList.forEach(illness => {
                    const { illnessId } = illness;

                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${illness.illnessName}</td>
                        <td style="white-space: normal;">${illness.description}</td>
                        <td>${illness.ruleCount}</td>
                        <td class="actions-cell">
                            <button class="btn btn-warning edit-disease-btn" style="padding: 0.5rem;" data-illness-id="${illnessId}" title="Edit"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg></button>
                            <button class="btn btn-danger delete-disease-btn" style="padding: 0.5rem;" data-illness-id="${illnessId}" title="Delete"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg></button>
                        </td>
                    `;
                    row.querySelector(table.actionBtns.edit).addEventListener('click', async (e) => {
                        if (illness) {
                            const result = await services.apiService.getDiseaseById(illnessId);

                            if (!result) {
                                return;
                            }

                            services.eventHandlers.populateDiseaseOnForm(result);
                            diseaseModal(true, true);
                        }
                    });
                    row.querySelector(table.actionBtns.delete).addEventListener('click', async (e) => {
                        if (confirm('Are you sure to delete this illness?')) {
                            const result = await services.apiService.removeIllness(illnessId);

                            if (!result) {
                                return;
                            }

                            await eventHandlers.renderIllnessTable();
                        }
                    });
                    $diseasesTableBody.append(row);
                });

                stateHolders.illnessPaginatedResult.totalPages = result.totalPages;
                this.createPaginations();
            },
            populateDiseaseOnForm: function (illness) {
                const { commandQueries } = stateHolders;
                const { fields } = elementHolders.forms.illness;

                const {
                    illnessId,
                    illnessName,
                    description,
                    rules
                } = illness ?? {};

                commandQueries.saveIllness.illnessId = illnessId;
                commandQueries.saveIllness.illnessName = illnessName;
                commandQueries.saveIllness.description = description;
                commandQueries.saveIllness.rules = rules ?? [];

                $(fields.illnessId).val(illnessId);
                $(fields.illnessName).val(illnessName);
                $(fields.description).val(description);

                this.constructSelectedRules();
            },
            constructSelectedRules: function () {
                const { forms } = elementHolders;
                const { commandQueries, tempSelectedRules, symptoms } = stateHolders;
                const diseaseRules = commandQueries.saveIllness.rules;

                const rulesToPopulates = diseaseRules.map(rule => {
                    const transformedRule = {
                        symptomId: rule.symptomId,
                        symptomName: symptoms.find(s => s.symptomId == rule.symptomId)?.symptomName ?? "",
                        condition: rule.condition,
                        days: rule.days,
                        weight: rule.weight
                    }

                    tempSelectedRules.push(transformedRule);

                    return transformedRule;
                });

                const $ruleFieldsContainer = $(forms.illness.rulesContainer);

                $ruleFieldsContainer.empty();
                $ruleFieldsContainer.append(rulesToPopulates.map(symptom => this.createRuleFieldConfig(symptom)));

                this.populateSymptomSelect();
            },
            renderDiseaseRules: function () {
            },
            createRulesConditionOption: function (selectedCondition) {
                const $select = $(`<select class="form-select rule-symptom-condition-select" style="width: 10rem;">`);

                const $options = stateHolders.ruleConditions.map(
                    c => `<option value="${c.id}" ${c.id == selectedCondition ? "selected" : ""}>${c.text}</option>`).join("");

                $select.append($options);

                return $select[0].outerHTML;
            },
            createRulesWeightOption: function (selectedWeight) {
                const $select = $(`<select class="form-select rule-symptom-weight-select" style="width: 10rem;">`);

                const $options = stateHolders.ruleWeights.map(
                    c => `<option value="${c.id}" ${c.id == selectedWeight ? "selected" : ""}>${c.text}</option>`).join("");

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
                        ${this.createRulesConditionOption(data.condition)}
                        <input type="number" value="${data.days}" min="1" class="form-input rule-symptom-days" style="width: 6rem;">
                        ${this.createRulesWeightOption(data.weight)}
                        <button class="remove-rule-btn" style="background:none; border:none; color:var(--danger-color); cursor:pointer; font-size:1.5rem; line-height:1;">&times;</button>
                    `;
                return ruleRow;
            },
            addRule: function () {
                const { forms } = elementHolders;
                const $ddSymptom = $(forms.illness.ddSymptom);
                const symptomId = parseInt($ddSymptom.val());

                if (symptomId == 0) return;

                const symptomName = $ddSymptom.find("option:selected").text();

                const symptom = {
                    symptomId: symptomId,
                    symptomName: symptomName,
                    condition: 1,
                    days: 1
                }

                const $ruleFieldsContainer = $(forms.illness.rulesContainer);

                $ruleFieldsContainer.prepend(this.createRuleFieldConfig(symptom));
                stateHolders.tempSelectedRules.push(symptom);

                this.populateSymptomSelect();
            },
            populateSymptomSelect: function () {
                const { forms } = elementHolders;
                const $ddSymptom = $(forms.illness.ddSymptom);
                $ddSymptom.val("");
                $ddSymptom.empty();

                const $defaultOption = $(`<option value="0">-- Select Symptom -- </option>`);
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
                const { fields } = forms.illness;

                $(fields.illnessId).val("");
                $(fields.illnessName).val("");
                $(fields.description).val("");

                const $ruleFieldsContainer = $(forms.illness.rulesContainer);

                $ruleFieldsContainer.empty();

                stateHolders.commandQueries.saveIllness = {
                    illnessId: 0,
                    illnessName: "",
                    description: "",
                    rules: []
                };
                stateHolders.tempSelectedRules = [];
            },
            getIllnessRules: function () {
                const $diseaseSymptomRules = $("#disease-rules-container").find(".rule-symptom-config-container");
                const rules = [...$diseaseSymptomRules].map(field => {
                    const symptomId = parseInt($(field).data("symptomId"));
                    const condition = parseInt($(field).find(".rule-symptom-condition-select").val());
                    const days = parseInt($(field).find(".rule-symptom-days").val());
                    const weight = parseInt($(field).find(".rule-symptom-weight-select").val());

                    return {
                        symptomId,
                        condition,
                        days,
                        weight
                    }
                });

                return rules;
            },
            handleOnSaveDisease: async function () {
                const { commandQueries, tempSelectedRules } = stateHolders;
                const { fields } = elementHolders.forms.illness;

                commandQueries.saveIllness.rules = this.getIllnessRules();
                const valid = this.validateDiseaseFields();

                if (!valid) return;

                const result = await services.apiService.saveDisease(commandQueries.saveIllness);
                if (!result) {
                    return;
                }

                await this.renderIllnessTable();

                this.toggleIllnessModal(false);
                this.resetDiseaseStates();
                $("#disease-rules-container").empty();
            },
            validateDiseaseFields: function () {
                const { commandQueries } = stateHolders;
                let result = true;

                result = ValidateInput(commandQueries.saveIllness.illnessName, "Disease Name is required.");
                if (!result) return false;

                result = ValidateInput(commandQueries.saveIllness.rules, "Rules Fields is required. Please add config at least 1");
                if (!result) return false;

                return result;
            },
            setSymptomsList: async function () {
                const response = await services.apiService.getIllnessSymptoms();
                stateHolders.symptoms = response ?? []
            },
            setRuleConditions: async function () {
                const response = await services.apiService.getRuleConditions() ?? [];
                stateHolders.ruleConditions = response.map(x => ({ id: x.id, text: x.conditionName }));
            },
            setWeightRules: async function () {
                const response = await services.apiService.getWeightRules() ?? [];
                stateHolders.ruleWeights = response.map(x => ({ id: x.id, text: x.name }));
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
                        eventHandlers.toggleIllnessModal(true, false);
                        eventHandlers.resetDiseaseStates();
                        eventHandlers.populateSymptomSelect();
                    }
                );

                registerEvent(
                    buttons.exportRulesBtn,
                    "click",
                    function (e) {
                        eventHandlers.handleOnExportRules();
                    }
                );
            },
            searchEvent: function () {
                const { searchSection } = elementHolders;

                registerEvent(
                    searchSection.searchBtn,
                    "click",
                    function () {
                        const searchValue = $(searchSection.searchInput).val();
                        stateHolders.searchParams.searchText = searchValue;
                        stateHolders.searchParams.pageNumber = 1;
                        services.eventHandlers.renderIllnessTable();
                    }
                );

                registerEvent(
                    searchSection.searchBtn,
                    "input",
                    function () {
                        const searchValue = $(searchSection.searchInput).val();
                        stateHolders.searchParams.searchText = searchValue;
                    }
                );

                registerEvent(
                    searchSection.paginationSelect,
                    "change",
                    function (event) {
                        const value = event.target.value;
                        stateHolders.searchParams.pageNumber = value;
                        services.eventHandlers.renderIllnessTable();
                    }
                );
            },
            illnessEvent: function () {
                const { forms } = elementHolders;
                const { eventHandlers } = services;
                const { commandQueries } = stateHolders;

                registerEvent(
                    forms.illness.closeBtn,
                    "click",
                    function (e) {
                        eventHandlers.toggleIllnessModal(false);
                        eventHandlers.resetDiseaseStates();
                        eventHandlers.populateSymptomSelect();
                    }
                );

                registerEvent(
                    forms.illness.saveBtn,
                    "click",
                    function (e) {
                        eventHandlers.handleOnSaveDisease();
                    }
                );

                registerEvent(
                    forms.illness.btnAddIllnessRule,
                    "click",
                    function (e) {
                        eventHandlers.addRule();
                    }
                );

                registerEvent(
                    forms.illness.rulesContainer,
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
                    forms.illness.fields.illnessName,
                    "input",
                    function (e) {
                        const $target = $(e.target);
                        commandQueries.saveIllness.illnessName = $target.val(); 
                    }
                );

                registerEvent(
                    forms.illness.fields.description,
                    "input",
                    function (e) {
                        const $target = $(e.target);
                        commandQueries.saveIllness.description = $target.val(); 
                    }
                );
            }
        },
        apiService: {
            getRuleConditions: async function () {
                return await apiFetch(URLS.getRuleConditions);
            },
            getIllnessSymptoms: async function () {
                return await apiFetch(URLS.getIllnessSymptoms);
            },
            getIllnessList: async function () {
                return await apiFetch(URLS.getIllnessList, { params: stateHolders.searchParams });
            },
            getDiseaseById: async function (illnessId) {
                return await apiFetch(
                    URLS.getIllnessById,
                    {
                        params: {
                            id: illnessId
                        }
                    }    
                );
            },
            saveDisease: async function (command) {
                return await apiFetch(
                    URLS.saveIllness,
                    {
                        method: "POST",
                        body: command
                    }
                );
            },
            removeIllness: async function (illnessId) {
                return await apiFetch(
                    URLS.deleteIllnessById,
                    {
                        method: "DELETE",
                        params: {
                            id: illnessId,
                        }
                    });
            },
            getWeightRules: async function () {
                return await apiFetch(URLS.getWeightRules);
            },
            getExportRulesConfig: async function () {
                return await apiFetch(URLS.getExportRulesConfig);
            }
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();