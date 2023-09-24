<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Forms.aspx.vb" Inherits="Forms" %>

<%@ MasterType VirtualPath="~/MasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="card card-header form-group pt-3 pb-2">

        <div class="form-group">
            <a class="form-group" style="font-size: 17px;" href="Applicant.aspx" role="button"><i class="fa fa-list" aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;Back</a>
        </div>

        <div class="card card-success form-group">
            <div class="card-body row pb-1">
                <!--NAME SEARCH-->
                <div class="col-md-4 has-feedback">
                    <label>Search name</label>
                    <div class="input-group">
                        <input type="search" id="txt_searchName" class="form-control form-control-md" placeholder="Firstname / Middlename / Lastname" runat="server" onchange="generateApplicant(this);">
                        <div class="input-group-append">
                            <button type="button" class="btn btn-md btn-default" runat="server" onchange="generateApplicant(this);">
                                <i class="fa fa-search"></i>
                            </button>
                        </div>
                    </div>
                </div>

                <!--APPLICANT REGISTERED DROP DOWN LIST-->
                <div class="form-group col-md-8">
                    <label>Applicant</label>
                    <select id="dl_info" class="form-control select2" runat="server" onchange="autofill(this)">
                        <option disabled selected hidden>...</option>
                    </select>
                </div>
            </div>
        </div>


        <div class="card card-success form-group">
            <div class="card-header">
                <h1 class="card-title" style="font-weight: bolder; font-size: 22px">Applicant Form</h1>
            </div>

            <div class="card-body pt-1">


                <%--hidden fields for refID--%>
                <input type="hidden" id="hdnRefID" runat="server" />

                <!--FAMILY INFORMATION-->
                <div class="row card-body col-md-12 pt-3 pb-1">
                    <h1 class="card-body pl-2 my-0 py-0" style="font-weight: bolder; font-size: 15px">Family Background</h1>
                </div>

                <div class="row card-body col-md-12 pt-2 pb-4 m-0">
                    <table class="table table-bordered" id="familyMembersTable">
                        <thead>
                            <tr>
                                <th class="text-center">#</th>
                                <th>Full Name</th>
                                <th>Birth Date</th>
                                <th>Occupancy Status</th>
                                <th>Family Income</th>
                                <th class="text-center">Number of Dependent</th>
                            </tr>
                        </thead>
                        <tbody id="familyMembersTableBody"></tbody>
                    </table>
                </div>


                <div class="card-body form-group col-md-12 py-0">
                    <hr class="m-0 p-0" />
                </div>

                <!--HOUSEHOLD HEAD INFORMATION
                <div class="row card-body col-md-12 pt-2 pb-2">
                    <h1 class="card-body pl-2 my-0 py-0" style="font-weight: bolder; font-size: 20px">Household Head Information</h1>
                </div>-->

                <div class="row form-group m-0 p-0">
                    <div class="card-body form-group col-md-4 pt-2 m-0">
                        <label for="txt_fName">First Name</label>
                        <input type="text" id="txt_fName" class="form-control required" placeholder="First Name" runat="server" disabled>
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 m-0">
                        <label>Middle Name</label>
                        <input type="text" id="txt_mName" class="form-control" placeholder="Middle Name" minlenght="2" runat="server" disabled>
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 m-0">
                        <label>Last Name</label>
                        <input type="text" id="txt_lName" class="form-control required" placeholder="Last Name" runat="server" disabled>
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 m-0">
                        <label>Suffix</label>
                        <select id="dl_suffix" class="form-control select2" runat="server" disabled>
                            <option value="">Suffix</option>
                            <option value="Jr">Jr</option>
                            <option value="Sr">Sr</option>
                            <option value="I">I</option>
                            <option value="II ">II </option>
                            <option value="III ">III </option>
                        </select>
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 m-0">
                        <label>Birthday</label>
                        <div class="input-group date lastNow" id="lastpublishdate" data-target-input="nearest">
                            <input type="text" class="form-control datetimepicker-input required" placeholder="Birthday" data-target="#lastpublishdate" id="bDate" runat="server" disabled />
                            <div class="input-group-append" data-target="#lastpublishdate" data-toggle="datetimepicker">
                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                            </div>
                        </div>
                    </div>

                    <%--CIVIL STATUS--%>
                    <div class="card-body form-group col-md-4 pt-2 m-0">
                        <label>Civil Status</label>
                        <select id="dl_civilStatus" class="form-control select2 required" runat="server">
                            <option value="">Civil Status</option>
                            <option value="Single">Single</option>
                            <option value="Married">Married</option>
                            <option value="Widowed">Widowed</option>
                            <option value="Divorced">Divorced</option>
                        </select>
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                        <label>Contact Number</label>
                        <input type="text" id="txt_contactNum" class="form-control required" placeholder="Contact Number" runat="server">
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                        <label>Voter's ID Number</label>
                        <input type="text" id="txt_Voters" class="form-control required" placeholder="Voter's ID Number" runat="server">
                    </div>

                    <div class="card-body form-group col-md-4 pt-2 pb-2 m-0">
                        <label>Purok/Sitio/Subdivision</label>
                        <select id="dl_psID" runat="server" class="form-control required select2">
                            <option value="">-</option>
                        </select>
                    </div>

                    <div class="card-body form-group col-md-6 pt-3 m-0">
                        <label>Address</label>
                        <textarea id="txt_address" class="form-control" rows="4" placeholder="Short Address" style="height: 38px; min-height: 38px;" spellcheck="false" runat="server"></textarea>
                    </div>

                    <div class="card-body form-group col-md-6 pt-3 m-0">
                        <label>Remarks</label>
                        <textarea id="txt_remarks" class="form-control" rows="4" placeholder="Remarks" style="height: 38px; min-height: 38px;" spellcheck="false" runat="server"></textarea>
                    </div>
                </div>

                <div class="card-body form-group col-md-12 py-0">
                    <hr class="m-0 p-0" />
                </div>

                <div class="row form-group pt-0 m-0 p-0">
                    <div class="card-body form-group col-md-4 m-0 pt-0">
                        <label>Select Category</label>
                        <select id="dl_category" class="form-control required select2" runat="server">
                            <option disabled selected hidden>Select Category</option>
                            <option value="MGBL">MGBL</option>
                            <option value="LGU">LGU</option>
                            <option value="NATIONAL">National</option>
                        </select>
                    </div>
                </div>
            </div>
        </div>

        <!--BUTTONS-->
        <div class="card-footer bg-transparent p-0 mb-2 form-group">
            <!--Save Button-->
            <button type="button" id="save_btn" class="btn bg-gradient-success float-right pr-5 pl-5">
                <i class="fa fa-save"></i>&nbsp;&nbsp;&nbsp;Save
            </button>

            <div class="form-group">
                <a class="form-group" style="font-size: 17px;" href="Applicant.aspx" role="button"><i class="fa fa-list" aria-hidden="true"></i>&nbsp;&nbsp;&nbsp;Back</a>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modal_confirm">
        <div class="modal-dialog mt-5">
            <div class="modal-content">
                <div class="modal-body">
                    <div class="form-group">
                        <h1 class="modal-title" style="font-weight: bold; font-size: x-large">Have you verified that the information in the form is accurate?</h1>
                    </div>
                </div>

                <div class="modal-footer justify-content-between">
                    <button type="button" class="btn btn-outline-info" data-dismiss="modal">Close</button>
                    <button id="modal_save" type="button" class="btn bg-gradient-success pr-4 pl-4" onclick="if (!validateForm('required')) return;" onserverclick="Save_Click" runat="server">
                        <i class="fa fa-save"></i>&nbsp;&nbsp;&nbsp;Save
                    </button>
                </div>
            </div>
        </div>
    </div>


    <script>
        //for debug purposes
        /*onchange = "logSelectedValue(this)"*/
        function logSelectedValue(selectElement) {
            var selectedValue = selectElement.value;
            console.log("Selected Value: " + selectedValue);
        }

        $('#save_btn').on("click", function () {
            if (!validateForm('required')) {
                systemMsg(0, 'Please fill in the required fields.');
                return;
            }

            var cpsID = $('#<%=dl_info.ClientID%>').val();

            console.log("Calling CheckUniqueness with cpsID: " + cpsID);

            $.ajax({
                type: "POST",
                url: "Forms.aspx/CheckUniqueness",
                data: JSON.stringify({ cpsID: cpsID }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    if (r.d == 0) {
                        systemMsg(0, 'This individual is already an applicant.');
                    } else {
                        $('#modal_confirm').modal('show');
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    systemMsg(0, 'Error during uniqueness validation.');
                }
            });
        });


        function generateApplicant(obj) {
            var ddlReg = $('#<%=dl_info.ClientID%>');
            ddlReg.empty().append('<option selected="selected" value="">Please Select Applicant</option>').val('').trigger('change');

            if (obj.value != '') {
                $.ajax({
                    type: "POST",
                    url: documentName + "/populateApplicant",
                    data: "{name: '" + obj.value + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        ddlReg.empty().append('<option selected="selected" value="">Please Select Applicant</option>');
                        $.each(r.d, function () {
                            ddlReg.append($("<option></option>").val(this['Value']).html(this['Text']));
                        });
                        ddlReg.focus();
                    }
                });
            }
        }

        function autofill(obj) {
            var hdnRefID = document.getElementById('<%= hdnRefID.ClientID %>');
            hdnRefID.value = obj.value;

            var cpsID = $('#<%=dl_info.ClientID%>').val();

            if (obj.value != '') {
                $.ajax({
                    type: "POST",
                    url: documentName + "/autoFill",
                    data: "{ID: '" + obj.value + "' }",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (a) {
                        var result = a.d;

                        //console.log("Selected element content: ", $('#familyMembersTable tbody').html());
                        //console.log("Family members HTML: ", result.familyMembersHTML);
                        console.log("cpsID: " + cpsID);


                        $('#<%=dl_category.ClientID%>').val(result.category);
                        $('#<%=txt_fName.ClientID%>').val(result.firstName);
                        $('#<%=txt_mName.ClientID%>').val(result.middleName);
                        $('#<%=txt_lName.ClientID%>').val(result.lastName);
                        $('#<%=dl_suffix.ClientID%>').val(result.suffix).trigger('change');
                        $('#<%=dl_civilStatus.ClientID%>').val(result.civilStatus).trigger('change');
                        $('#<%=bDate.ClientID%>').val(result.birthDay);
                        $('#<%=txt_Voters.ClientID%>').val(result.vin);
                        $('#<%=txt_contactNum.ClientID%>').val(result.contactNum);
                        $('#<%=dl_psID.ClientID%>').val(result.psID).trigger('change');
                        $('#<%=txt_address.ClientID%>').val(result.shortAdd);
                        $('#<%=txt_remarks.ClientID%>').val(result.remarks);

                        $('#familyMembersTable tbody').html(result.familyMembersHTML);
                    }
                });
            }
        }




    </script>

</asp:Content>