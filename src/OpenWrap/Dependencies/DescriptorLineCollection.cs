﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenWrap.Dependencies
{
    public class DescriptorLineCollection : IEnumerable<IPackageDescriptorLine>
    {
        readonly Dictionary<string, List<GenericDescriptorLine>> _byName = new Dictionary<string, List<GenericDescriptorLine>>(StringComparer.OrdinalIgnoreCase);
        readonly List<GenericDescriptorLine> _headers = new List<GenericDescriptorLine>();

        public DescriptorLineCollection(IEnumerable<KeyValuePair<string, string>> descriptorLines)
        {
            foreach (var value in descriptorLines)
                Append(value.Key, value.Value);
        }

        public DescriptorLineCollection()
        {
        }

        public IPackageDescriptorLine Append(string name, string value)
        {
            var header = new GenericDescriptorLine(name, value);
            if (!_headers.Contains(header))
                _headers.Add(header);
            GetOrAddValueList(name).Add(header);
            return header;
        }

        public void Remove(string name)
        {
            var headers = GetOrAddValueList(name);
            _headers.RemoveRange(headers);
            headers.Clear();
        }

        public void Remove(string name, string value)
        {
            var headerValue = new GenericDescriptorLine(name, value);
            _headers.Remove(headerValue);
            GetOrAddValueList(name).Remove(headerValue);
        }

        public void Set(string name, string value)
        {
            // want to keep the order of headers, need to choose the first found one
            var allHeadersWithSameName = _headers.Where(x => x.Name.EqualsNoCase(name))
                    .ToList();

            var entryToEdit = allHeadersWithSameName.FirstOrDefault();
            var presentHeaderIndex = _headers.IndexOf(entryToEdit);
            var toRemove = allHeadersWithSameName.ToList();

            _headers.RemoveRange(toRemove);

            var headerValues = GetOrAddValueList(name);
            headerValues.Clear();

            var header = new GenericDescriptorLine(name, value);

            if (presentHeaderIndex == -1)
                _headers.Add(header);
            else
                _headers.Insert(presentHeaderIndex, header);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IPackageDescriptorLine> GetEnumerator()
        {
            foreach (var val in _headers)
                yield return val;
        }

        List<GenericDescriptorLine> GetOrAddValueList(string name)
        {
            List<GenericDescriptorLine> values;
            if (!_byName.TryGetValue(name, out values))
                _byName[name] = values = new List<GenericDescriptorLine>();
            return values;
        }
    }
}