using UnityEngine;
using XNode;

namespace Gaza
{

    public static class GazaNodeTool
    {
        /// <summary>
        /// 获取端口连接节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Node GetNodeByField(this Node node, string fieldName)
        {
            if (!node.HasPort(fieldName))
            {
                Debug.LogWarning(fieldName + " 不存在");
                return null;
            }

            var port = node.GetPort(fieldName);
            if (!port.IsConnected)
            {
                Debug.LogWarning(fieldName + " 未连接");
                return null;
            }

            return port.Connection.node;
        }

        /// <summary>
        /// 获取端口属性的值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetValuesByField(this Node node, string fieldName)
        {
            return node.GetType().GetField(fieldName).GetValue(node);
        }
    }

}