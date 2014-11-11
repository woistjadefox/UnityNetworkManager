﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Goga.UnityNetwork {

    public enum RPCType {
        Bool, Int, Float, String
    }

    public class RPCObject {

        public RPCType type;
        public bool boolData;
        public int intData;
        public float floatData;
        public string stringData;

        public RPCObject(RPCType type) {
            this.type = type;
        }
    }

    [RequireComponent(typeof(NetworkView))]

    public class NetObject : MonoBehaviour {

        private Manager uNet;
        public string playerGuid;

        private StackFrame _frame;
        private string _callerMethod;

        public Dictionary<string, System.Action> lastRpcCalls = new Dictionary<string, System.Action>();

        void Awake() {

            Network.isMessageQueueRunning = false;
        }

        void Start() {
            this.uNet = FindObjectOfType<Manager>();
            Network.isMessageQueueRunning = true;
        }

        // set the owner of the object
        [RPC]
        public void SetOwner(string guid) {

            this.playerGuid = guid;
        }

        public Manager GetManager() {
            return this.uNet;
        }

        public bool IsMine() {

            if (Network.player.guid == this.playerGuid) {
                return true;
            }

            return false;
        }


        public bool RoleObserverBase(RPCObject state, int senderID, bool broadcastToAll, bool allowLocalAction) {

            // get caller method name
            _frame = new StackFrame(2);
            _callerMethod = _frame.GetMethod().Name;


            if (Network.isClient && this.IsMine() && senderID != 1) {

                switch (state.type) {
                    case RPCType.Bool:

                        this.lastRpcCalls[_callerMethod] = new System.Action(() => {
                            networkView.RPC(_callerMethod, RPCMode.Server, state.boolData, 0);
                        });

                        this.lastRpcCalls[_callerMethod](); // run rpc
                        break;

                    case RPCType.Int:

                        this.lastRpcCalls[_callerMethod] = new System.Action(() => {
                            networkView.RPC(_callerMethod, RPCMode.Server, state.intData, 0);
                        });

                        this.lastRpcCalls[_callerMethod](); // run rpc
                        break;

                    case RPCType.Float:

                        this.lastRpcCalls[_callerMethod] = new System.Action(() => {
                            networkView.RPC(_callerMethod, RPCMode.Server, state.floatData, 0);
                        });

                        this.lastRpcCalls[_callerMethod](); // run rpc
                        break;

                    case RPCType.String:

                        this.lastRpcCalls[_callerMethod] = new System.Action(() => {
                            networkView.RPC(_callerMethod, RPCMode.Server, state.stringData, 0);
                        });

                        this.lastRpcCalls[_callerMethod](); // run rpc
                        break;
                }


                if (allowLocalAction) {
                    return true;
                }
            }

            if (Network.isServer) {

                if (broadcastToAll) {

                    UnityEngine.Debug.Log("server broadcasting: " + _callerMethod);

                    switch (state.type) {
                        case RPCType.Bool:
                            networkView.RPC(_callerMethod, RPCMode.OthersBuffered, state.boolData, 1);
                            break;

                        case RPCType.Int:
                            networkView.RPC(_callerMethod, RPCMode.OthersBuffered, state.intData, 1);
                            break;

                        case RPCType.Float:
                            networkView.RPC(_callerMethod, RPCMode.OthersBuffered, state.floatData, 1);
                            break;

                        case RPCType.String:
                            networkView.RPC(_callerMethod, RPCMode.OthersBuffered, state.stringData, 1);
                            break;
                    }
                }

                return true;
            }

            if (senderID == 1 && !this.IsMine()) {
                return true;
            }

            if (!allowLocalAction && senderID == 1) {
                return true;

            }

            return false;
        }

        public bool RoleObserver(bool state, int senderID, bool broadcastToAll, bool allowLocalAction) {

            RPCObject obj = new RPCObject(RPCType.Bool);
            obj.boolData = state;

            return this.RoleObserverBase(obj, senderID, broadcastToAll, allowLocalAction);
        }

        public bool RoleObserver(int state, int senderID, bool broadcastToAll, bool allowLocalAction) {
            
            RPCObject obj = new RPCObject(RPCType.Int);
            obj.intData = state;

            return this.RoleObserverBase(obj, senderID, broadcastToAll, allowLocalAction);
        }

        public bool RoleObserver(float state, int senderID, bool broadcastToAll, bool allowLocalAction) {
            
            RPCObject obj = new RPCObject(RPCType.Float);
            obj.floatData = state;

            return this.RoleObserverBase(obj, senderID, broadcastToAll, allowLocalAction);
        }

        public bool RoleObserver(string state, int senderID, bool broadcastToAll, bool allowLocalAction) {

            RPCObject obj = new RPCObject(RPCType.String);
            obj.stringData = state;

            return this.RoleObserverBase(obj, senderID, broadcastToAll, allowLocalAction);
        }

    }
}
