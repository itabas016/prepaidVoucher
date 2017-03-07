﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AscCashValid.ASC_CASH_VALIDSoap")]
    public interface ASC_CASH_VALIDSoap {
        
        // CODEGEN: Generating message contract since element name accountno from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/user_auth", ReplyAction="*")]
        PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponse user_auth(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/user_auth", ReplyAction="*")]
        System.Threading.Tasks.Task<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponse> user_authAsync(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/user_auth_amt", ReplyAction="*")]
        PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponse user_auth_amt(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/user_auth_amt", ReplyAction="*")]
        System.Threading.Tasks.Task<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponse> user_auth_amtAsync(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class user_authRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="user_auth", Namespace="http://tempuri.org/", Order=0)]
        public PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequestBody Body;
        
        public user_authRequest() {
        }
        
        public user_authRequest(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class user_authRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string accountno;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string usercode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string password;
        
        public user_authRequestBody() {
        }
        
        public user_authRequestBody(string accountno, string usercode, string password) {
            this.accountno = accountno;
            this.usercode = usercode;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class user_authResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="user_authResponse", Namespace="http://tempuri.org/", Order=0)]
        public PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponseBody Body;
        
        public user_authResponse() {
        }
        
        public user_authResponse(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class user_authResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string error;
        
        public user_authResponseBody() {
        }
        
        public user_authResponseBody(string error) {
            this.error = error;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class user_auth_amtRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="user_auth_amt", Namespace="http://tempuri.org/", Order=0)]
        public PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequestBody Body;
        
        public user_auth_amtRequest() {
        }
        
        public user_auth_amtRequest(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class user_auth_amtRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string accountno;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string usercode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public double amt;
        
        public user_auth_amtRequestBody() {
        }
        
        public user_auth_amtRequestBody(string accountno, string usercode, string password, double amt) {
            this.accountno = accountno;
            this.usercode = usercode;
            this.password = password;
            this.amt = amt;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class user_auth_amtResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="user_auth_amtResponse", Namespace="http://tempuri.org/", Order=0)]
        public PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponseBody Body;
        
        public user_auth_amtResponse() {
        }
        
        public user_auth_amtResponse(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class user_auth_amtResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string error;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public float amount;
        
        public user_auth_amtResponseBody() {
        }
        
        public user_auth_amtResponseBody(string error, float amount) {
            this.error = error;
            this.amount = amount;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ASC_CASH_VALIDSoapChannel : PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ASC_CASH_VALIDSoapClient : System.ServiceModel.ClientBase<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap>, PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap {
        
        public ASC_CASH_VALIDSoapClient() {
        }
        
        public ASC_CASH_VALIDSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ASC_CASH_VALIDSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ASC_CASH_VALIDSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ASC_CASH_VALIDSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponse PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap.user_auth(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest request) {
            return base.Channel.user_auth(request);
        }
        
        public string user_auth(string accountno, string usercode, string password) {
            PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest inValue = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest();
            inValue.Body = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequestBody();
            inValue.Body.accountno = accountno;
            inValue.Body.usercode = usercode;
            inValue.Body.password = password;
            PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponse retVal = ((PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap)(this)).user_auth(inValue);
            return retVal.Body.error;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponse> PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap.user_authAsync(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest request) {
            return base.Channel.user_authAsync(request);
        }
        
        public System.Threading.Tasks.Task<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authResponse> user_authAsync(string accountno, string usercode, string password) {
            PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest inValue = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequest();
            inValue.Body = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_authRequestBody();
            inValue.Body.accountno = accountno;
            inValue.Body.usercode = usercode;
            inValue.Body.password = password;
            return ((PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap)(this)).user_authAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponse PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap.user_auth_amt(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest request) {
            return base.Channel.user_auth_amt(request);
        }
        
        public string user_auth_amt(string accountno, string usercode, string password, double amt, out float amount) {
            PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest inValue = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest();
            inValue.Body = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequestBody();
            inValue.Body.accountno = accountno;
            inValue.Body.usercode = usercode;
            inValue.Body.password = password;
            inValue.Body.amt = amt;
            PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponse retVal = ((PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap)(this)).user_auth_amt(inValue);
            amount = retVal.Body.amount;
            return retVal.Body.error;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponse> PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap.user_auth_amtAsync(PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest request) {
            return base.Channel.user_auth_amtAsync(request);
        }
        
        public System.Threading.Tasks.Task<PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtResponse> user_auth_amtAsync(string accountno, string usercode, string password, double amt) {
            PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest inValue = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequest();
            inValue.Body = new PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.user_auth_amtRequestBody();
            inValue.Body.accountno = accountno;
            inValue.Body.usercode = usercode;
            inValue.Body.password = password;
            inValue.Body.amt = amt;
            return ((PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid.ASC_CASH_VALIDSoap)(this)).user_auth_amtAsync(inValue);
        }
    }
}
