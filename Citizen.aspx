<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Citizen.aspx.vb" Inherits="Departments" %>
<%@ MasterType  virtualPath="~/MasterPage.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <div class="row">
       <div class="col-12">
            <div class="card">
                <!--<div class="card-header">
                    <button type="button" data-target="#modal_add" data-toggle="modal" id="btn_addnew" class="btn bg-gradient-primary">
                                    <i class="ace-icon fa fa-plus-circle bigger-110"></i>
                                    Add New
                    </button>
                </div>-->

                <div class="card-body">
                    <div class="table-responsive ">
                        <table id="example1" class="table table-bordered table-striped basicTable">
                          <thead>
                              <tr>
                                  <th>ID</th>
                                  <th>Full Name</th>
                                  <th>Birthday</th>
                                  <th>Sex</th>
                                  <th>Age</th>
                                  <th>Address</th>
                                  <th>Year of Resicency</th>
                                  <th>Status</th>
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
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h4 class="modal-title" runat="server" id="h4_title">Add New</h4>
              <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div class="modal-body">
                <div class="form-group" >                        
                    <label>First Name</label>
                    <div>
                        <input id="txt_fName" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, 'fName')"/>
                    </div> 
                </div> 

                <div class="form-group" >                        
                    <label>Middle Name</label>
                    <div>
                        <input id="txt_mName" maxlength="45" class="form-control" runat="server"/>
                    </div> 
                </div> 

                <div class="form-group" >                        
                    <label>Last Name</label>
                    <div>
                        <input id="txt_lName" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, 'lName')"/>
                    </div> 
                </div> 

                <div class="form-group" >                        
                    <label>BirthDate</label>
                    <div class="input-group date startNow" id="lastpublishdate" data-target-input="nearest">
                            <input type="text" class="form-control datetimepicker-input requiredPage" data-target="#lastpublishdate" id="bDay" runat="server" />
                        <div class="input-group-append" data-target="#lastpublishdate" data-toggle="datetimepicker">
                            <div class="input-group-text"><i class="fa fa-calendar"></i></div>
                        </div> 

                    </div>
                </div>

                <div class="form-group" >                        
                    <label>Address</label>
                    <div>
                        <input id="Address" maxlength="45" class="form-control required" runat="server" onchange="validateEntry(this, 'sAddress')"/>
                    </div> 
                </div> 

                <div class="form-group" id="div_psID" runat ="server">                        
                    <label>Purok/Sitio/Subdivision</label>
                    <select id="dl_psID" runat="server" class="form-control select2">
                        <option value= "">-</option>
                    </select>
                </div> 

                <div class="form-group" id="div_status" runat ="server" visible="false">                        
                    <label>Status</label>
                    <select id="dl_status" runat="server" class="form-control select2">
                        <option value= "1">Active</option>
                        <option value= "0">Inactive</option>
                    </select>
                </div> 
            </div>
            <div class="modal-footer justify-content-between">
              <button type="button" class="btn btn-outline-dark" data-dismiss="modal">Close</button>
              <button type="button" class="btn btn-outline-success" onclick="if (!validateForm('required')) return;" onserverclick="Save_Click" runat ="server">Save changes</button>
            </div>
          </div>
        </div>
    </div> 
    
    
</asp:Content>

