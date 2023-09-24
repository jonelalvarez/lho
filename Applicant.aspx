<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Applicant.aspx.vb" Inherits="Applicant" %>
<%@ MasterType  virtualPath="~/MasterPage.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="card card-header">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <a class="btn btn-success pr-4 pl-4" href="Forms.aspx" role="button"><i class="fa fa-user-plus"></i>&nbsp;&nbsp;&nbsp;Add Applicant</a>
                    </div>
                    <div class="row card-body pt-1">
                        <div class="col-md-4 col-xs-12">
                            <label>Application Date</label>
                            <div class="input-group">
                                <div class="input-group-prepend">
                                    <span class="input-group-text">
                                        <i class="far fa-calendar-alt"></i>
                                    </span>
                                </div>
                                <input type="text" class="form-control float-right" id="reportrange" onchange="generateTable();">
                            </div>
                        </div>
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label">Category</label>
                            <select id="dl_category" class="form-control select2" onchange="generateTable();">
                                <option value="">All</option>
                                <option value="1">MGBL HOMES</option>
                                <option value="2">NHA LGU</option>
                                <option value="3">NHA NATIONAL</option>
                                <option value="4">VMBM HOMES</option>
                            </select>
                        </div>          
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label">Application Status</label>
                            <select id="dl_applicationstatus" class="form-control select2" onchange="generateTable();">
                                <option value="">All</option>
                                <option value="3">Approved</option>
                                <option value="1">For Requirements</option>
                                <option value="2">For Approval</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
       <div class="row">
        <div class="col-12">
            <div class="card">

                <div class="card-body">
                    <div class="table-responsive ">
                        <table id="example1" class="table table-bordered table-striped basicTable">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Full Name</th>
                                    <th>Address</th>
                                    <th>Category</th>
                                    <th>Application Status</th>
                                    <th>Application Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_list" runat="server"></asp:PlaceHolder>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>


    
     <div class="modal fade" id="modal_add">
         <div class="modal-dialog modal-xl">
          <div class="modal-content">
            <div class="modal-header">
              <h4 class="modal-title" runat="server" id="h4_title">Add New</h4>
              <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div class="modal-body">
                <div class="card-body row form-group m-0 p-0">
                    <div class="card-body form-group col-md-12" id="app_status" runat="server">
                        <label>Application Status</label>
                        <select id="application_status" runat="server" class="form-control select2">
                            <option value="1">For Requirements</option>
                            <option value="2">For Approval</option>
                            <option value="3">Approved</option>
                            <option value="4">Disapproved</option>
                        </select>
                    </div>
                </div> 
                <div class="form-group col-md-12 py-0">
                    <hr class="m-0 p-0">
                </div>

                <!-- HOUSEHOLD HEAD -->

                <div class="form-group col-md-12" style="font-weight: bolder;" >
                    <h5><b> HOUSEHOLD HEAD</b> </h5>
                </div>
                <div class="card-body row form-group m-0 p-0">
                    <div class="card-body form-group col-md-4 p-2">
                        <label>First Name</label>
                        <div>
                            <input id="txt_fName" maxlength="45" class="form-control required" runat="server"  />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Middle Name</label>
                        <div>
                            <input id="txt_mName" maxlength="45" class="form-control" runat="server" disabled="" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Last Name</label>
                        <div>
                            <input id="txt_lName" maxlength="45" class="form-control required" runat="server"  />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Suffix</label>
                        <select id="div_suffix" class="form-control placeholder" runat="server" disabled="">
                            <option value=""></option>
                            <option value="Jr">Jr</option>
                            <option value="Sr">Sr</option>
                        </select>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Birthdate</label>
                        <div class="input-group date lastNow" id="lastpublishdate" data-target-input="nearest">
                            <input type="text" class="form-control datetimepicker-input requiredPage" data-target="#lastpublishdate" id="bDay" runat="server"  />
                            <div class="input-group-append" data-target="#lastpublishdate" data-toggle="datetimepicker">
                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                            </div>
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Civil Status</label>
                        <select id="dl_civil" class="form-control placeholder" runat="server" disabled="">
                            <option value="1">Single</option>
                            <option value="2">Married</option>
                            <option value="3">Widowed</option>
                            <option value="4">Separated</option>
                        </select>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Contact Number</label>
                        <div>
                            <input id="div_contact" maxlength="45" class="form-control required" runat="server" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Voters ID</label>
                        <div>
                            <input id="div_voters" maxlength="45" class="form-control required" runat="server"  />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Years of Residency</label>
                        <div>
                            <input id="div_residency" maxlength="45" class="form-control required" runat="server" />
                        </div>
                    </div>
                </div> 
                <div class="card-body row form-group m-0 p-0">
                    <div class="card-body form-group col-md-6 p-2">
                        <label>Address</label>
                        <div>
                            <input id="Address" maxlength="45" class="form-control required" runat="server" />
                        </div>
                    </div>

                    <div class="card-body form-group col-md-6 p-2" id="div_psID" runat="server">
                        <label>Purok/Sitio/Subdivision</label>
                        <select id="dl_psID" runat="server" class="form-control required select2">
                            <option value="">-</option>
                        </select>
                    </div>
                </div>
                <div class="card-body form-group col-md-12 py-0">
                    <hr class="m-0 p-0">
                </div>

                <!-- SPOUSE INFORMATION -->

              <%--  <div class="form-group col-md-12" style="font-weight: bolder;">
                    <h5><b>SPOUSE'S INFORMATION</b></h5>
                </div>
                <div class="row form-group m-0 p-0">
                    <div class="card-body form-group col-md-4 p-2">
                        <label>First Name</label>
                        <div>
                            <input id="SpouseFname" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, 'FirstName')" disabled="" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Middle Name</label>
                        <div>
                            <input id="SpouseMname" maxlength="45" class="form-control" runat="server" disabled="" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Last Name</label>
                        <div>
                            <input id="SpouseLname" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, 'LastName')" disabled="" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Suffix</label>
                        <select id="dl_suffix" class="form-control placeholder" runat="server" onchange="validateEntry(this, 'dl_suffix')" disabled="">
                            <option value=""></option>
                            <option value="Jr">Jr</option>
                            <option value="Sr">Sr</option>
                        </select>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Birthdate</label>
                        <div class="input-group date lastNow" id="div_lastpublishdate" data-target-input="nearest">
                            <input type="text" class="form-control datetimepicker-input requiredPage" data-target="#lastpublishdate" id="Text6" runat="server" disabled="" />
                            <div class="input-group-append" data-target="#lastpublishdate" data-toggle="datetimepicker">
                                <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                            </div>
                        </div>
                    </div>
                     <div class="card-body form-group col-md-4 p-2">
                        <label>Civil Status</label>
                        <select id="Select2" class="form-control placeholder" runat="server" onchange="validateEntry(this)" disabled="">
                            <option value="1">Single</option>
                            <option value="2">Married</option>
                            <option value="3">Widowed</option>
                            <option value="4">Separated</option>
                        </select>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Contact Number</label>
                        <div>
                            <input id="spouse_contact" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, '')" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Voters ID</label>
                        <div>
                            <input id="Text1" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, '')" />
                        </div>
                    </div>
                    <div class="card-body form-group col-md-4 p-2">
                        <label>Years of Residency</label>
                        <div>
                            <input id="Text4" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, '')" />
                        </div>
                    </div>
                </div> --%>

               
                <div class="card-body row form-group m-0 p-0">
                    <div class="form-group col-md-12">
                        <label>Remarks</label>
                        <textarea class="form-control" rows="3" placeholder="Remarks..."></textarea>
                    </div>
                </div>
            </div>
            <div class="modal-footer justify-content-between">
              <button type="button" class="btn btn-outline-dark" data-dismiss="modal">Close</button>
              <button type="button" class="btn btn-outline-success" onclick="if (!validateForm('required')) return;" onserverclick="Save_Click" runat ="server">Save changes</button>
            </div>
          </div>
        </div>
        </div>


    <script>
        function generateTable() {
            var dateRange = $('#reportrange').val();
            var category = $('#dl_category').val();
            var status = $('#dl_applicationstatus').val();

            if (dateRange && category !== null && status !== null) {
                // Extract the start and end dates from the DateRangePicker input
                var dateParts = dateRange.split(' - ');
                var startDate = moment(dateParts[0], 'MM/DD/YYYY').format('YYYY-MM-DD');
                var endDate = moment(dateParts[1], 'MM/DD/YYYY').format('YYYY-MM-DD');

                var requestData = {
                    _from: startDate,
                    _to: endDate,
                    status: status,
                    category: category
                };

                // Log the requestData to the console for debugging
                console.log("Request Data:", requestData);

                $.ajax({
                    type: "POST",
                    url: "Applicant.aspx/filterDataTable",
                    data: JSON.stringify(requestData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        console.log("Response Data:", r);
                        // Get the DataTable instance
                        var dataTable = $('#example1').DataTable();
                        dataTable.clear()
                        dataTable.rows.add($(r.d)).draw();
                    }
                });
            }
        }

        $('#reportrange').daterangepicker(
            {
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },
                startDate: moment().subtract(29, 'days'),
                endDate: moment()
            },
            function (start, end) {
                $('#reportrange').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'))
            }
        )
    </script>
</asp:Content>